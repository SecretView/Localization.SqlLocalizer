// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

namespace SecretCollect.Localization.SqlLocalizer.Data
{
    /// <summary>
    /// The status of a record
    /// </summary>
    public enum RecordStatus : byte
    {
        /// <summary>
        /// New record, mostly meaning not yet translated
        /// </summary>
        New = 20,
        /// <summary>
        /// Translated using a translation service, could use some human verification
        /// </summary>
        MachineTranslated = 40,
        /// <summary>
        /// Translated but needs some extra checking
        /// </summary>
        NeedsValidation = 60,
        /// <summary>
        /// Fully translated and checked
        /// </summary>
        HumanTranslated = 80
    }
}
