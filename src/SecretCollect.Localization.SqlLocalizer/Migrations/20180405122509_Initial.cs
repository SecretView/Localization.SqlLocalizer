// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SecretCollect.Localization.SqlLocalizer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalizationKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Base = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportedCultures",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsSupported = table.Column<bool>(nullable: false),
                    LanguageCode2 = table.Column<string>(maxLength: 2, nullable: false),
                    RegionCode2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportedCultures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationRecords",
                columns: table => new
                {
                    LocKey_Id = table.Column<Guid>(nullable: false),
                    Culture_Id = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    UpdatedTimestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationRecords", x => new { x.LocKey_Id, x.Culture_Id });
                    table.ForeignKey(
                        name: "FK_LocalizationRecords_SupportedCultures_Culture_Id",
                        column: x => x.Culture_Id,
                        principalTable: "SupportedCultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocalizationRecords_LocalizationKeys_LocKey_Id",
                        column: x => x.LocKey_Id,
                        principalTable: "LocalizationKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationKeys_Base_Key",
                table: "LocalizationKeys",
                columns: new[] { "Base", "Key" },
                unique: true,
                filter: "[Key] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationRecords_Culture_Id",
                table: "LocalizationRecords",
                column: "Culture_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SupportedCultures_LanguageCode2_RegionCode2",
                table: "SupportedCultures",
                columns: new[] { "LanguageCode2", "RegionCode2" },
                unique: true,
                filter: "[LanguageCode2] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalizationRecords");

            migrationBuilder.DropTable(
                name: "SupportedCultures");

            migrationBuilder.DropTable(
                name: "LocalizationKeys");
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
