// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System.Globalization;

namespace SecretCollect.Localization.SqlLocalizer.Internal
{
    internal static class CacheKeyProvider
    {
        public static string GetBaseCacheKey(CultureInfo culture, string baseName) => $"Culture={culture.Name};BaseKey={baseName}";

        public static string GetSpecificCacheKey(CultureInfo culture, string baseName, string key, bool useFallBackCulture) => GetSpecificCacheKey(culture.Name, baseName, key, useFallBackCulture);
        public static string GetSpecificCacheKey(string cultureName, string baseName, string key, bool useFallBackCulture) => $"Culture={cultureName};BaseKey={baseName};MainKey={key};FallBack={useFallBackCulture}";
    }
}
