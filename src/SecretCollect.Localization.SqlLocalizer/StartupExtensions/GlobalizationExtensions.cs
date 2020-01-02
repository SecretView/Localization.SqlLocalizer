// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SecretCollect.Localization.SqlLocalizer;
using SecretCollect.Localization.SqlLocalizer.Data;
using SecretCollect.Localization.SqlLocalizer.Internal.Configurations;
using SecretCollect.Localization.SqlLocalizer.Settings;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Static extension class to add globalization to the servicecollection
    /// </summary>
    public static class GlobalizationExtensions
    {
        /// <summary>
        /// Add globalization services to the <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">The service collection to add services to</param>
        /// <param name="optionsAction">The <see cref="DbContextOptionsBuilder"/> action that is used to add the <see cref="LocalizationContext"/></param>
        /// <returns>The same <see cref="IServiceCollection"/> as the input for chaining purposes</returns>
        public static IServiceCollection AddGlobalization(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            services.AddMemoryCache();
            services.AddDbContext<LocalizationContext>(optionsAction);
            services.TryAddSingleton<IConfigureOptions<GlobalizationOptions>, ConfigureGlobalizationSettings>();
            services.TryAddSingleton<IConfigureOptions<RequestLocalizationOptions>, ConfigureRequestLocalizationOptions>();
            services.TryAddSingleton<IStringLocalizerFactory, SqlStringLocalizerFactory>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(SqlStringLocalizer<>));
            services.TryAddTransient<Preloader>();
            return services;
        }
    }
}
