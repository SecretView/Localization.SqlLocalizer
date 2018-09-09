// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System;

namespace SecretCollect.Localization.SqlLocalizer.Data
{
    /// <summary>
    /// A POCO representing a localization
    /// </summary>
    public class LocalizationRecord
    {
        /// <summary>
        /// The localized text
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The status of the record
        /// </summary>
        public RecordStatus Status { get; set; } = RecordStatus.New;
        /// <summary>
        /// The key that this record belongs to
        /// </summary>
        public LocalizationKey LocalizationKey { get; set; }
        /// <summary>
        /// The culture of the localization
        /// </summary>
        public SupportedCulture Culture { get; set; }
        /// <summary>
        /// The last time this localization is used
        /// </summary>
        /// <remarks>
        /// Usefull for find out which records become outdated
        /// </remarks>
        public DateTime LastUsed { get; set; }
    }
}
