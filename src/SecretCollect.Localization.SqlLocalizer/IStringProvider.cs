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
        /// Get a localized string for the provided key and culture
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="culture">The culture</param>
        /// <param name="updateLastUsed">Indicate if the LastUsed property of the record needs to be updated</param>
        /// <param name="useFallBackCulture">Use the fallback culture when retrieving the culture</param>
        /// <returns>A localized string</returns>
        string GetString(string key, CultureInfo culture, bool updateLastUsed, bool useFallBackCulture);

        /// <summary>
        /// Get all localization keys
        /// </summary>
        /// <returns>A collection of localization keys</returns>
        IEnumerable<string> GetAllResourceKeys();
    }
}
