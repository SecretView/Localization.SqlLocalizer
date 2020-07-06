// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecretCollect.Localization.SqlLocalizer.Data;
using SecretCollect.Localization.SqlLocalizer.Internal;
using SecretCollect.Localization.SqlLocalizer.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SecretCollect.Localization.SqlLocalizer
{
    /// <summary>
    /// An implementation of <see cref="IStringProvider"/> using a <see cref="LocalizationContext"/>
    /// </summary>
    public class SqlStringProvider : IStringProvider
    {
        private const string ALL_RESOURCE_STRINGS = nameof(ALL_RESOURCE_STRINGS);

        private readonly string _cacheKeyAll;

        private readonly IMemoryCache _memoryCache;
        private readonly string _baseName;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<GlobalizationOptions> _globalizationOptions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseName">The basename of the localization key</param>
        /// <param name="serviceScopeFactory">Used for accessing the current <see cref="LocalizationContext"/></param>
        /// <param name="memoryCache">The memory cache used for caching the localization lookup</param>
        /// <param name="globalizationOptions">The options used to check the cache time</param>
        public SqlStringProvider(string baseName, IServiceScopeFactory serviceScopeFactory, IMemoryCache memoryCache, IOptions<GlobalizationOptions> globalizationOptions)
        {
            _baseName = baseName ?? throw new ArgumentNullException(nameof(baseName));
            _cacheKeyAll = $"{ALL_RESOURCE_STRINGS}_{_baseName}";
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _globalizationOptions = globalizationOptions ?? throw new ArgumentNullException(nameof(globalizationOptions));
        }

        /// <inheritdoc />
        public string GetString(string key, CultureInfo culture, bool updateLastUsed, bool useFallBackCulture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var cacheKey = CacheKeyProvider.GetSpecificCacheKey(culture, _baseName, key, useFallBackCulture);

            var localization = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _globalizationOptions.Value.CacheTime;

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<LocalizationContext>();
                    return _getLocalizationString(context, culture, _baseName, key, updateLastUsed, useFallBackCulture);
                }
            });

            if (localization == null)
                _memoryCache.Remove(cacheKey);
            return localization;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetAllResourceKeys()
        {
            var resourceStrings = _memoryCache.GetOrCreate(_cacheKeyAll, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _globalizationOptions.Value.CacheTime;

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<LocalizationContext>();

                    var records = context.LocalizationKeys
                        .Where(r => r.Base == _baseName)
                        .Select(r => r.Key)
                        .ToArray();

                    if (!records.Any())
                        return null;

                    return records;
                }
            });

            if (resourceStrings == null)
                _memoryCache.Remove(_cacheKeyAll);

            return resourceStrings ?? Array.Empty<string>();
        }

        private string _getLocalizationString(LocalizationContext context, CultureInfo culture, string baseKey, string mainKey, bool updateLastUsed, bool useFallBackCulture)
        {
            var cultureName = culture.Name;
            var dbCulture = context.SupportedCultures
                .Include(c => c.FallbackCulture)
                .SingleOrDefault(c => c.Name == cultureName && c.IsSupported);
            if (dbCulture == null)
                return null;

            return _getLocalizationString(context, dbCulture, baseKey, mainKey, updateLastUsed, useFallBackCulture);
        }

        private string _getLocalizationString(LocalizationContext context, SupportedCulture culture, string baseKey, string mainKey, bool updateLastUsed, bool useFallBackCulture)
        {
            var dbKey = _getOrCreateKey(context, baseKey, mainKey);

            var localization = context.LocalizationRecords
                .Where(r => r.LocalizationKey.Id == dbKey.Id && r.Culture.Id == culture.Id)
                .SingleOrDefault();

            if (useFallBackCulture && string.IsNullOrWhiteSpace(localization?.Text) && culture.FallbackCulture != null)
            {
                context.Entry(culture.FallbackCulture).Reference(x => x.FallbackCulture).Load();
                return _getLocalizationString(context, culture.FallbackCulture, baseKey, mainKey, updateLastUsed, useFallBackCulture);
            }

            if (updateLastUsed && localization != null)
            {
                localization.LastUsed = DateTime.Now;
                context.SaveChanges();
            }

            return localization?.Text;
        }

        private static LocalizationKey _getOrCreateKey(LocalizationContext context, string baseKey, string mainKey)
        {
            var dbKey = context.LocalizationKeys.SingleOrDefault(k => k.Base == baseKey && k.Key == mainKey);
            if (dbKey == null)
            {
                // Key doesn't exist, create!
                dbKey = new LocalizationKey()
                {
                    Base = baseKey,
                    Key = mainKey,
                    Comment = LocalizationKey.DEFAULT_COMMENT
                };
                try
                {
                    context.LocalizationKeys.Add(dbKey);
                    context.SaveChanges();
                }
                catch (DbUpdateException ex) when (( ex.InnerException as Microsoft.Data.SqlClient.SqlException )?.Number == 2601)
                {
                    // Key already exists, (concurrent request probably added it): reload entity.
                    context.Entry(dbKey).State = EntityState.Detached;
                    dbKey = context.LocalizationKeys.SingleOrDefault(k => k.Base == baseKey && k.Key == mainKey);
                }
            }

            return dbKey;
        }
    }
}
