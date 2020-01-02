// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretCollect.Localization.SqlLocalizer.Migrations
{
    public partial class CultureFallback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FallbackCultureId",
                table: "SupportedCultures",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportedCultures_FallbackCultureId",
                table: "SupportedCultures",
                column: "FallbackCultureId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportedCultures_SupportedCultures_FallbackCultureId",
                table: "SupportedCultures",
                column: "FallbackCultureId",
                principalTable: "SupportedCultures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportedCultures_SupportedCultures_FallbackCultureId",
                table: "SupportedCultures");

            migrationBuilder.DropIndex(
                name: "IX_SupportedCultures_FallbackCultureId",
                table: "SupportedCultures");

            migrationBuilder.DropColumn(
                name: "FallbackCultureId",
                table: "SupportedCultures");
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member