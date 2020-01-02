// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using SecretCollect.Localization.SqlLocalizer.Data;
using SimpleMvc.Controllers;
using SimpleMvc.Models;
using System.Collections.Generic;

namespace SimpleMvc
{
    public static class DatabaseSeeder
    {
        public static void InitializeDatabase(LocalizationContext context)
        {
            var cultureNL = new SupportedCulture() { IsSupported = true, Name = "nl" };
            var cultureEN = new SupportedCulture() { IsSupported = true, Name = "en" };
            context.SupportedCultures.Add(cultureNL);
            context.SupportedCultures.Add(cultureEN);
            context.SaveChanges();

            var collection = new Dictionary<LocalizationKey, (SupportedCulture Culture, string Value)[]>()
                {
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "JANUARY" }, new [] { (cultureNL, "Januari"), (cultureEN, "January") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "FEBRUARY" }, new [] { (cultureNL, "Februari"), (cultureEN, "February") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "MARCH" }, new [] { (cultureNL, "Maart"), (cultureEN, "March") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "APRIL" }, new [] { (cultureNL, "April"), (cultureEN, "April") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "MAY" }, new [] { (cultureNL, "Mei"), (cultureEN, "May") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "JUNE" }, new [] { (cultureNL, "Juni"), (cultureEN, "June") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "JULY" }, new [] { (cultureNL, "Juli"), (cultureEN, "July") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "AUGUST" }, new [] { (cultureNL, "Augustus"), (cultureEN, "August") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "SEPTEMBER" }, new [] { (cultureNL, "September"), (cultureEN, "September") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "OCTOBER" }, new [] { (cultureNL, "Oktober"), (cultureEN, "October") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "NOVEMBER" }, new [] { (cultureNL, "November"), (cultureEN, "November") } },
                    { new LocalizationKey() { Base = typeof(Months).FullName, Key = "DECEMBER" }, new [] { (cultureNL, "December"), (cultureEN, "December") } },
                    { new LocalizationKey() { Base = typeof(SimpleController).FullName, Key = "HELLO_WORLD" }, new [] { (cultureNL, "Hallo wereld!"), (cultureEN, "Hello world!") } },
                    { new LocalizationKey() { Base = typeof(SimpleController).FullName, Key = "HELLO_PERSON" }, new [] { (cultureNL, "Hallo {0}!"), (cultureEN, "Hello {0}!") } },
                };

            foreach (var item in collection)
            {
                context.LocalizationKeys.Add(item.Key);
                context.SaveChanges();
                foreach (var (Culture, Value) in item.Value)
                    context.LocalizationRecords.Add(new LocalizationRecord()
                    {
                        Culture = Culture,
                        LocalizationKey = item.Key,
                        Status = RecordStatus.HumanTranslated,
                        Text = Value
                    }).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                context.SaveChanges();
            }
        }
    }
}
