// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SecretCollect.Localization.SqlLocalizer.Data;
using SecretCollect.Localization.SqlLocalizer.Internal;
using SecretCollect.Localization.SqlLocalizer.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecretCollect.Localization.SqlLocalizer
{
    /// <summary>
    /// Preloader that can start caching all localizations into the memorycache
    /// </summary>
    public class Preloader
    {
        private readonly IMemoryCache _memoryCache;
        private readonly LocalizationContext _context;
        private readonly IOptions<GlobalizationOptions> _globalizationOptions;

        /// <summary>
        /// Constructor for the <see cref="Preloader"/>
        /// </summary>
        /// <param name="memoryCache">The memory cache used for caching the localization lookup</param>
        /// <param name="context">The context used to lookup the localizations</param>
        /// <param name="globalizationOptions">The options used to check the cache time</param>
        public Preloader(IMemoryCache memoryCache, LocalizationContext context, IOptions<GlobalizationOptions> globalizationOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _globalizationOptions = globalizationOptions ?? throw new ArgumentNullException(nameof(globalizationOptions));
        }

        /// <summary>
        /// Cache all keys that use the specified basekey
        /// </summary>
        /// <param name="baseKey">The basekey to preload</param>
        /// <param name="absoluteExpirationRelativeToNow">The absolute expiration relative to now</param>
        public void Cache(string baseKey, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            if (baseKey == null)
                throw new ArgumentNullException(baseKey);

            absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? _globalizationOptions?.Value?.CacheTime ?? TimeSpan.FromDays(30);

            var records = _context.LocalizationRecords
                .Where(r => r.LocalizationKey.Base == baseKey)
                .Where(r => r.Culture.IsSupported)
                .Select(r => new { r.Text, r.LocalizationKey.Base, r.LocalizationKey.Key, CultureName = r.Culture.Name })
                .ToArray();

            foreach (var record in records)
            {
                var cacheKey = CacheKeyProvider.GetSpecificCacheKey(record.CultureName, record.Base, record.Key, false);
                _memoryCache.Set(cacheKey, record.Text, absoluteExpirationRelativeToNow.Value);
            }
        }

        /// <summary>
        /// Cache all the localizations in the database into the memorycache
        /// </summary>
        /// <param name="absoluteExpirationRelativeToNow">The absolute expiration relative to now</param>
        public void CacheAll(TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var baseKeys = _context.LocalizationKeys.Select(k => k.Base).Distinct().ToArray();
            foreach (var baseKey in baseKeys)
                Cache(baseKey, absoluteExpirationRelativeToNow);
        }

        /// <summary>
        /// Cache all keys from which a record was recently used
        /// </summary>
        /// <param name="maxAge">The maximum age of the record LastUsed param</param>
        /// <param name="absoluteExpirationRelativeToNow">The absolute expiration relative to now</param>
        public void CacheRecentlyUsed(TimeSpan maxAge, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var lastUsed = DateTime.UtcNow - maxAge;
            var records = _context.LocalizationKeys
                .Where(k => k.Records.Any(r => r.LastUsed >= lastUsed))
                .SelectMany(k => k.Records)
                .Select(r => new { r.Text, r.LocalizationKey.Base, r.LocalizationKey.Key, CultureName = r.Culture.Name })
                .ToArray();

            foreach (var record in records)
            {
                var cacheKey = CacheKeyProvider.GetSpecificCacheKey(record.CultureName, record.Base, record.Key, false);
                _memoryCache.Set(cacheKey, record.Text, absoluteExpirationRelativeToNow.Value);
            }
        }
    }
}
