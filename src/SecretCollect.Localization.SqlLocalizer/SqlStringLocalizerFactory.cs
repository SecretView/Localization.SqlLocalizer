// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecretCollect.Localization.SqlLocalizer.Settings;
using System;
using System.Reflection;

namespace SecretCollect.Localization.SqlLocalizer
{
    /// <summary>
    /// Implementation of <see cref="IStringLocalizerFactory"/>
    /// </summary>
    public class SqlStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<GlobalizationOptions>  _globalizationOptions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> used for logger creation</param>
        /// <param name="contextAccessor">The <see cref="IHttpContextAccessor"/> used when creating new <see cref="SqlStringLocalizer"/></param>
        /// <param name="globalizationOptions">The options used when creating <see cref="SqlStringLocalizer"/></param>
        /// <param name="memoryCache">The <see cref="IMemoryCache"/> used when creating new <see cref="SqlStringLocalizer"/></param>
        public SqlStringLocalizerFactory(ILoggerFactory loggerFactory, IHttpContextAccessor contextAccessor, IOptions<GlobalizationOptions> globalizationOptions, IMemoryCache memoryCache)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _globalizationOptions = globalizationOptions ?? throw new ArgumentNullException(nameof(globalizationOptions));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>
        /// Get the resource prefix for a certain type
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> to get the prefix for</param>
        /// <returns>The prefix</returns>
        protected virtual string GetResourcePrefix(TypeInfo typeInfo)
        {
            if (typeInfo == null)
                throw new ArgumentNullException(nameof(typeInfo));

            return typeInfo.FullName;
        }

        /// <summary>
        /// Get the resource prefix for a certain type
        /// </summary>
        /// <param name="baseResourceName">The base resource name</param>
        /// <param name="baseNamespace">The base namespace</param>
        /// <returns>The resource prefix</returns>
        protected virtual string GetResourcePrefix(string baseResourceName, string baseNamespace)
        {
            if (string.IsNullOrEmpty(baseResourceName))
                throw new ArgumentNullException(nameof(baseResourceName));

            if (string.IsNullOrEmpty(baseNamespace))
                throw new ArgumentNullException(nameof(baseNamespace));

            baseResourceName = baseNamespace + "." + _trimPrefix(baseResourceName, baseNamespace + ".");

            return baseResourceName;
        }

        /// <inheritdoc />
        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
                throw new ArgumentNullException(nameof(resourceSource));

            var typeInfo = resourceSource.GetTypeInfo();

            var baseName = GetResourcePrefix(typeInfo);

            return CreateSqlStringLocalizer(baseName);
        }

        /// <inheritdoc />
        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName == null)
                throw new ArgumentNullException(nameof(baseName));

            if (location == null)
                throw new ArgumentNullException(nameof(location));

            baseName = GetResourcePrefix(baseName, location);

            return CreateSqlStringLocalizer(baseName);
        }

        /// <summary>
        /// Create a SqlStringLocalizer for a certain basename
        /// </summary>
        /// <param name="baseName">The basename</param>
        /// <returns>A new <see cref="SqlStringLocalizer"/></returns>
        protected virtual SqlStringLocalizer CreateSqlStringLocalizer(string baseName)
        {
            var logger = _loggerFactory.CreateLogger<SqlStringLocalizer>();
            return new SqlStringLocalizer(baseName, _contextAccessor, _memoryCache, _globalizationOptions, logger);
        }

        private static string _trimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
                return name.Substring(prefix.Length);

            return name;
        }
    }
}
