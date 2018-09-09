// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SecretCollect.Localization.SqlLocalizer.Data;

namespace SecretCollect.Localization.SqlLocalizer.Internal
{
    internal class DbInitializer : IDesignTimeDbContextFactory<LocalizationContext>
    {
        public LocalizationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LocalizationContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=LocalizationsMigrations;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new LocalizationContext(optionsBuilder.Options);
        }
    }
}
