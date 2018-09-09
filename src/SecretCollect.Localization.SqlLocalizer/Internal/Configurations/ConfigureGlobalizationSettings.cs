// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecretCollect.Localization.SqlLocalizer.Data;
using SecretCollect.Localization.SqlLocalizer.Settings;
using System;

namespace SecretCollect.Localization.SqlLocalizer.Internal.Configurations
{
    internal class ConfigureGlobalizationSettings : IConfigureOptions<GlobalizationOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ConfigureGlobalizationSettings(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(GlobalizationOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var configuration = provider.GetRequiredService<IConfiguration>();
                using (var dbContext = provider.GetRequiredService<LocalizationContext>())
                {
                    var globalizationSection = configuration.GetSection("Globalization");

                    options.CacheTime = TimeSpan.FromMinutes(globalizationSection.GetValue<double>("CacheTimeInMinutes"));
                    options.CookieName = globalizationSection.GetValue<string>("CookieName");
                    options.CookieDomain = globalizationSection.GetValue<string>("CookieDomain");
                }
            }
        }
    }
}
