// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.EntityFrameworkCore;
using SecretCollect.Localization.SqlLocalizer.Data;
using System.Linq;
using Xunit;

namespace SecretCollect.Localization.SqlLocalizer.UnitTests.Data
{
    public class LocalizationContextTests
    {
        [Fact]
        public void Add_writes_to_database()
        {
            var options = new DbContextOptionsBuilder<LocalizationContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

            using (var context = new LocalizationContext(options))
            {
                var culture = new SupportedCulture() { IsSupported = true, Name = "nl" };
                context.SupportedCultures.Add(culture);

                var key = new LocalizationKey() { Base = "Hello.World.Test", Key = "HELLO_WORLD" };
                context.LocalizationKeys.Add(key);

                context.LocalizationRecords.Add(new LocalizationRecord()
                {
                    Culture = culture,
                    LocalizationKey = key,
                    Status = RecordStatus.HumanTranslated,
                    Text = "Hallo wereld!"
                }).State = EntityState.Added;

                context.SaveChanges();
            }

            using (var context = new LocalizationContext(options))
            {
                Assert.Equal(1, context.SupportedCultures.Count());
                Assert.Equal("nl", context.SupportedCultures.Single().Name);
                Assert.Equal("Hallo wereld!", context.LocalizationRecords.Single().Text);
            }
        }
    }
}
