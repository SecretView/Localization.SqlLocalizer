// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SecretCollect.Localization.SqlLocalizer
{
    /// <summary>
    /// A sub class of <see cref="SqlStringLocalizer"/> which overrides the <see cref="CultureInfo"/> used in the lookups
    /// </summary>
    public class SqlStringWithCultureLocalizer : SqlStringLocalizer
    {
        private readonly CultureInfo _culture;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlStringProvider">The <see cref="IStringProvider"/> used for localization lookup</param>
        /// <param name="baseName">The basename of the keys</param>
        /// <param name="culture">The culture used in lookups</param>
        /// <param name="logger">The logger used</param>
        public SqlStringWithCultureLocalizer(IStringProvider sqlStringProvider, string baseName, CultureInfo culture, ILogger logger)
            : base(sqlStringProvider, baseName, logger)
        {
            _culture = culture;
        }

        /// <inheritdoc />
        public override LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var value = GetStringSafely(name, _culture);

                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _baseName);
            }
        }

        /// <inheritdoc />
        public override LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var format = GetStringSafely(name, _culture);
                var value = string.Format(_culture, format ?? name, arguments);

                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _baseName);
            }
        }

        /// <inheritdoc />
        public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            GetAllStrings(includeParentCultures, _culture);
    }
}
