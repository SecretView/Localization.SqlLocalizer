// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;

namespace SecretCollect.Localization.SqlLocalizer.Data
{
    /// <summary>
    /// A culture that is supported
    /// </summary>
    public class SupportedCulture
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Boolean indicating if this culture is used in the localizations
        /// </summary>
        public bool IsSupported { get; set; }

        /// <summary>
        /// The culture name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The <see cref="LocalizationRecord"/> for this culture
        /// </summary>
        public ICollection<LocalizationRecord> Records { get; set; }
    }
}
