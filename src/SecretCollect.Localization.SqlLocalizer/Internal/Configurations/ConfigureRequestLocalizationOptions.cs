// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecretCollect.Localization.SqlLocalizer.Data;
using SecretCollect.Localization.SqlLocalizer.Settings;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SecretCollect.Localization.SqlLocalizer.Internal.Configurations
{
    internal class ConfigureRequestLocalizationOptions : IConfigureOptions<RequestLocalizationOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ConfigureRequestLocalizationOptions(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(RequestLocalizationOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;

                var globalizationSettings = provider.GetRequiredService<IOptions<GlobalizationOptions>>().Value;

                var configuration = provider.GetRequiredService<IConfiguration>();
                using (var dbContext = provider.GetRequiredService<LocalizationContext>())
                {
                    options.SupportedCultures = configuration
                        .GetSection("Globalization")
                        .GetSection("SupportedCultures")
                        .Get<string[]>()
                        .Select(sc => new CultureInfo(sc))
                        .ToArray();
                    options.SupportedUICultures = dbContext.SupportedCultures
                            .Where(s => s.IsSupported)
                            .Select(s => s.Name)
                            .ToArray()
                            .Select(name => new CultureInfo(name))
                            .ToArray();
                    var defaultCulure = options.SupportedCultures.FirstOrDefault() ?? CultureInfo.CurrentCulture;
                    var defaultUICulture = options.SupportedUICultures.FirstOrDefault() ?? CultureInfo.CurrentUICulture;
                    options.DefaultRequestCulture = new RequestCulture(defaultCulure, defaultUICulture);

                    options.RequestCultureProviders = new List<IRequestCultureProvider>
                    {
                        new QueryStringRequestCultureProvider() { Options = options },
                        new CookieRequestCultureProvider() { Options = options, CookieName = globalizationSettings.CookieName },
                        new AcceptLanguageHeaderRequestCultureProvider() { Options = options }
                    };
                }
            }
        }
    }
}
