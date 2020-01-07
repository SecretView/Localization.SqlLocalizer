// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecretCollect.Localization.SqlLocalizer.Internal;
using SecretCollect.Localization.SqlLocalizer.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SecretCollect.Localization.SqlLocalizer
{
    /// <summary>
    /// An implementation of <see cref="IStringLocalizer"/> using a <see cref="SqlStringProvider"/>
    /// </summary>
    public class SqlStringLocalizer : IStringLocalizer
    {
        /// <summary>
        /// The baseName / key used for key lookup
        /// </summary>
        protected readonly string BaseName;
        private readonly IStringProvider _stringProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseName">The basename used for key lookup</param>
        /// <param name="serviceScopeFactory"><see cref="IServiceScopeFactory"/> used for creating the <see cref="SqlStringProvider"/></param>
        /// <param name="memoryCache"><see cref="IMemoryCache"/> used for creating the <see cref="SqlStringProvider"/></param>
        /// <param name="globalizationOptions"><see cref="GlobalizationOptions"/> options used for creating the <see cref="SqlStringProvider"/></param>
        /// <param name="logger">The <see cref="ILogger"/> used for logging</param>
        public SqlStringLocalizer(string baseName, IServiceScopeFactory serviceScopeFactory, IMemoryCache memoryCache, IOptions<GlobalizationOptions> globalizationOptions, ILogger logger)
            : this(new SqlStringProvider(baseName, serviceScopeFactory, memoryCache, globalizationOptions), baseName, logger)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stringProvider">The <see cref="IStringProvider"/> used for localization lookup</param>
        /// <param name="baseName">The basename used for key lookup</param>
        /// <param name="logger">The <see cref="ILogger"/> used for logging</param>
        public SqlStringLocalizer(IStringProvider stringProvider, string baseName, ILogger logger)
        {
            _stringProvider = stringProvider ?? throw new ArgumentNullException(nameof(stringProvider));
            BaseName = baseName ?? throw new ArgumentNullException(nameof(baseName));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public virtual LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var value = GetStringSafely(name, null, true);

                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: BaseName);
            }
        }

        /// <inheritdoc />
        public virtual LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var format = GetStringSafely(name, null, true);
                var value = string.Format(format ?? name, arguments);

                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: BaseName);
            }
        }

        /// <inheritdoc />
        public IStringLocalizer WithCulture(CultureInfo culture)
            => culture == null
                ? new SqlStringLocalizer(
                    _stringProvider,
                    BaseName,
                    _logger)
                : new SqlStringWithCultureLocalizer(
                    _stringProvider,
                    BaseName,
                    culture,
                    _logger);

        /// <inheritdoc />
        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);

        /// <summary>
        /// Get string based upon the provided culture or Current UI Culture
        /// </summary>
        /// <param name="name">The name of the key</param>
        /// <param name="culture">The culture to get the localization for</param>
        /// <param name="useFallbackCulture">Try a fallback culture if no localization is found.</param>
        /// <returns>A localized string</returns>
        protected string GetStringSafely(string name, CultureInfo culture, bool useFallbackCulture)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var keyCulture = culture ?? CultureInfo.CurrentUICulture;

            _logger.SearchedLocation(name, BaseName, keyCulture);

            return _stringProvider.GetString(name, keyCulture, updateLastUsed: true, useFallbackCulture);
        }

        /// <summary>
        /// Get all localized strings for a culture
        /// </summary>
        /// <param name="includeParentCultures">Bool indicating if the parent cultures should be used for the lookup</param>
        /// <param name="culture">The culture to get the localizations for</param>
        /// <returns>A collection of <see cref="LocalizedString"/></returns>
        protected IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            var resourceNames = _stringProvider.GetAllResourceKeys();

            foreach (var name in resourceNames)
            {
                var value = GetStringSafely(name, culture, includeParentCultures);
                yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: BaseName);
            }
        }
    }
}
