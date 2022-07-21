// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace SecretCollect.Localization.SqlLocalizer.MigrationStartup
{
    internal class CustomDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
            => serviceCollection.AddSingleton<IMigrationsCodeGenerator, DisableCS1591CSharpMigrationsGenerator>();
    }
}
