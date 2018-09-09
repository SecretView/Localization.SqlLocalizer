// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SecretCollect.Localization.SqlLocalizer.Migrations
{
    public partial class MergedLanguageAndRegionIntoName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupportedCultures_LanguageCode2_RegionCode2",
                table: "SupportedCultures");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SupportedCultures",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE [SupportedCultures] SET [Name] = CASE WHEN [RegionCode2] IS NULL OR [RegionCode2] = '' THEN [LanguageCode2] ELSE [LanguageCode2] + '-' + COALESCE([RegionCode2], '') END");

            migrationBuilder.DropColumn(
                name: "LanguageCode2",
                table: "SupportedCultures");

            migrationBuilder.DropColumn(
                name: "RegionCode2",
                table: "SupportedCultures");

            migrationBuilder.CreateIndex(
                name: "IX_SupportedCultures_Name",
                table: "SupportedCultures",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupportedCultures_Name",
                table: "SupportedCultures");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode2",
                table: "SupportedCultures",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegionCode2",
                table: "SupportedCultures",
                maxLength: 64,
                nullable: true);

            migrationBuilder.Sql("UPDATE [SupportedCultures] SET [LanguageCode2] = CASE WHEN CHARINDEX('-', [Name]) > 0 THEN SUBSTRING([Name], 1, CHARINDEX('-', [Name]) - 1) ELSE SUBSTRING([Name], 1, 2) END");
            migrationBuilder.Sql("UPDATE [SupportedCultures] SET [RegionCode2] = CASE WHEN CHARINDEX('-', [Name]) > 0 THEN SUBSTRING([Name], CHARINDEX('-', [Name]) + 1, LEN([Name]) - CHARINDEX('-', [Name])) ELSE NULL END");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SupportedCultures");

            migrationBuilder.CreateIndex(
                name: "IX_SupportedCultures_LanguageCode2_RegionCode2",
                table: "SupportedCultures",
                columns: new[] { "LanguageCode2", "RegionCode2" },
                unique: true,
                filter: "[LanguageCode2] IS NOT NULL");
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
