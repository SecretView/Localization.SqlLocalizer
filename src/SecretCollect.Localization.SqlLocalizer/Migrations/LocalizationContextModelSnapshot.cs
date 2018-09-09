﻿// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecretCollect.Localization.SqlLocalizer.Data;

namespace SecretCollect.Localization.SqlLocalizer.Migrations
{
    [DbContext(typeof(LocalizationContext))]
    partial class LocalizationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SecretCollect.Localization.SqlLocalizer.Data.LocalizationKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Base");

                    b.Property<string>("Comment");

                    b.Property<string>("Key")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Base", "Key")
                        .IsUnique()
                        .HasFilter("[Key] IS NOT NULL");

                    b.ToTable("LocalizationKeys");
                });

            modelBuilder.Entity("SecretCollect.Localization.SqlLocalizer.Data.LocalizationRecord", b =>
                {
                    b.Property<Guid>("LocKey_Id");

                    b.Property<Guid>("Culture_Id");

                    b.Property<DateTime>("LastUsed");

                    b.Property<byte>("Status");

                    b.Property<string>("Text");

                    b.Property<DateTime>("UpdatedTimestamp");

                    b.HasKey("LocKey_Id", "Culture_Id");

                    b.HasIndex("Culture_Id");

                    b.ToTable("LocalizationRecords");
                });

            modelBuilder.Entity("SecretCollect.Localization.SqlLocalizer.Data.SupportedCulture", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsSupported");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("SupportedCultures");
                });

            modelBuilder.Entity("SecretCollect.Localization.SqlLocalizer.Data.LocalizationRecord", b =>
                {
                    b.HasOne("SecretCollect.Localization.SqlLocalizer.Data.SupportedCulture", "Culture")
                        .WithMany("Records")
                        .HasForeignKey("Culture_Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SecretCollect.Localization.SqlLocalizer.Data.LocalizationKey", "LocalizationKey")
                        .WithMany("Records")
                        .HasForeignKey("LocKey_Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member