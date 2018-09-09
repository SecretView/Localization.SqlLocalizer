// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System;

namespace SecretCollect.Localization.SqlLocalizer.Settings
{
    /// <summary>
    /// Options POCO for globalization
    /// </summary>
    public class GlobalizationOptions
    {
        /// <summary>
        /// The domain for the localization cookie
        /// </summary>
        public string CookieDomain { get; set; }
        /// <summary>
        /// The name of the localization cookie
        /// </summary>
        public string CookieName { get; set; }
        /// <summary>
        /// The absolute expiration relative to now for the memorycache
        /// </summary>
        public TimeSpan CacheTime { get; set; } = TimeSpan.FromMinutes(15);
    }
}
