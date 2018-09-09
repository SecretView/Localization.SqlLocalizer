// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;

namespace SecretCollect.Localization.SqlLocalizer.Data
{
    /// <summary>
    /// The localization key
    /// </summary>
    public class LocalizationKey
    {
        /// <summary>
        /// Default comment to be added when this key is added because of a missing translation
        /// </summary>
        public const string DEFAULT_COMMENT = "System generated, key was missing.";

        /// <summary>
        /// Primary key
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Base of the key (grouping/namespace)
        /// </summary>
        public string Base{ get; set; }
        /// <summary>
        /// The key itself within the base (subkey/classname)
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Possible comment
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// All localizations for this key
        /// </summary>
        public ICollection<LocalizationRecord> Records { get; set; }
    }
}