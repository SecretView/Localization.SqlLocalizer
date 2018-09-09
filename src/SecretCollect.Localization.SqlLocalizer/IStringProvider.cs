// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Globalization;

namespace SecretCollect.Localization.SqlLocalizer
{
    /// <summary>
    /// Interface used to get strings based on a <see cref="CultureInfo"/>
    /// </summary>
    public interface IStringProvider
    {
        /// <summary>
        /// Get a translated string based upon a key and culture
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="culture">The culture</param>
        /// <returns>A localized string</returns>
        string GetString(string key, CultureInfo culture);
        /// <summary>
        /// Get all localized strings for a culture
        /// </summary>
        /// <param name="culture">The culture</param>
        /// <param name="throwOnMissing">Throw an exception when no localized strings are found for the given culture</param>
        /// <returns>A collection of localized strings</returns>
        IEnumerable<string> GetAllResourceStrings(CultureInfo culture, bool throwOnMissing);
    }
}
