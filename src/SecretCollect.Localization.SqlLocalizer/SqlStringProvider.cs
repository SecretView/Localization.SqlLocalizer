// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecretCollect.Localization.SqlLocalizer.Data;
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
        private readonly IMemoryCache _memoryCache;
        private readonly string _baseName;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<GlobalizationOptions> _globalizationOptions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseName">The basename of the localization key</param>
        /// <param name="contextAccessor">Used for accessing the current <see cref="LocalizationContext"/></param>
        /// <param name="memoryCache">The memory cache used for caching the localization lookup</param>
        /// <param name="globalizationOptions">The options used to check the cache time</param>
        public SqlStringProvider(string baseName, IHttpContextAccessor contextAccessor, IMemoryCache memoryCache, IOptions<GlobalizationOptions> globalizationOptions)
        {
            _baseName = baseName ?? throw new ArgumentNullException(nameof(baseName));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _globalizationOptions = globalizationOptions ?? throw new ArgumentNullException(nameof(globalizationOptions));
        }

        /// <inheritdoc />
        public string GetString(string key, CultureInfo culture) => GetString(key, culture, true);

        /// <summary>
        /// Get a localized string for the provided key and culture
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="culture">The culture</param>
        /// <param name="updateLastUsed">Indicate if the LastUsed property of the record needs to be updated</param>
        /// <returns>A localized string</returns>
        public string GetString(string key, CultureInfo culture, bool updateLastUsed)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var cacheKey = _getSpecificCacheKey(culture, _baseName, key);

            var localization = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _globalizationOptions.Value.CacheTime;

                var context = _contextAccessor.HttpContext.RequestServices.GetService<LocalizationContext>();
                return _getLocalizationString(context, culture, _baseName, key, updateLastUsed );
            });

            if (localization == null)
                _memoryCache.Remove(cacheKey);
            return localization;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetAllResourceStrings(CultureInfo culture, bool throwOnMissing)
        {
            var cacheKey = _getBaseCacheKey(culture);

            var resourceStrings = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _globalizationOptions.Value.CacheTime;
                var cultureName = culture.Name;
                var context = _contextAccessor.HttpContext.RequestServices.GetService<LocalizationContext>();

                if (!context.SupportedCultures.Any(c => c.Name == cultureName))
                    return null;

                var names = context.LocalizationKeys
                    .Where(k => k.Base == _baseName)
                    .Select(k => k.Key)
                    .ToArray();

                if (!names.Any())
                    return null;

                return names;
            });

            if (resourceStrings == null)
            {
                _memoryCache.Remove(cacheKey);
                if (throwOnMissing)
                    throw new Exception($"No localization keys found for culture {culture.Name} and base {_baseName}");
            }
            return resourceStrings;
        }

        private string _getLocalizationString(LocalizationContext context, CultureInfo culture, string baseKey, string mainKey, bool updateLastUsed)
        {
            var cultureName = culture.Name;
            var dbCulture = context.SupportedCultures.SingleOrDefault(c => c.Name == cultureName && c.IsSupported);
            if (dbCulture == null)
                return null;

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
                catch (DbUpdateException ex) when ((ex.InnerException as System.Data.SqlClient.SqlException)?.Number == 2601)
                {
                    // Key already exists, (concurrent request probably added it): reload entity.
                    context.Entry(dbKey).State = EntityState.Detached;
                    dbKey = context.LocalizationKeys.SingleOrDefault(k => k.Base == baseKey && k.Key == mainKey);
                }
            }

            var localization = context.LocalizationRecords
                .Where(r => r.LocalizationKey.Id == dbKey.Id && r.Culture.Id == dbCulture.Id)
                .SingleOrDefault();

            if (updateLastUsed && localization != null)
            {
                localization.LastUsed = DateTime.Now;
                context.SaveChanges();
            }

            return localization?.Text;
        }

        private string _getBaseCacheKey(CultureInfo culture) => $"Culture={culture.Name};BaseKey={_baseName}";

        private string _getSpecificCacheKey(CultureInfo culture, string @base, string key) => $"Culture={culture.Name};BaseKey={_baseName};MainKey={key}";
    }
}
