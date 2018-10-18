// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SecretCollect.Localization.SqlLocalizer.Data
{
    /// <summary>
    /// Database context for the localizations
    /// </summary>
    /// <remarks>
    /// To add a migration use:
    /// Add-Migration -Project SecretCollect.Localization.SqlLocalizer -StartupProject SecretCollect.Localization.SqlLocalizer.MigrationStartup -Context LocalizationContext
    /// </remarks>
    public class LocalizationContext : DbContext
    {
        private const string UPDATED_TIMESTAMP = "UpdatedTimestamp";

        /// <inheritdoc />
        public LocalizationContext(DbContextOptions<LocalizationContext> options)
            : base(options)
        { }

        /// <summary>
        /// The DbSet of <see cref="LocalizationRecord"/>
        /// </summary>
        public DbSet<LocalizationRecord> LocalizationRecords { get; set; }
        /// <summary>
        /// The DbSet of <see cref="LocalizationKey"/>
        /// </summary>
        public DbSet<LocalizationKey> LocalizationKeys { get; set; }
        /// <summary>
        /// The DbSet of <see cref="SupportedCulture"/>
        /// </summary>
        public DbSet<SupportedCulture> SupportedCultures { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SupportedCulture>().HasKey(s => s.Id);
            builder.Entity<SupportedCulture>().HasIndex(s => s.Name).IsUnique(true);
            builder.Entity<SupportedCulture>().Property(s => s.Name).IsRequired().HasMaxLength(64);

            builder.Entity<LocalizationKey>().HasKey(s => s.Id);
            builder.Entity<LocalizationKey>().HasIndex(s => new { s.Base, s.Key }).IsUnique(true).HasFilter("[Key] IS NOT NULL");
            builder.Entity<LocalizationKey>().Property(s => s.Key).IsRequired(true);
            builder.Entity<LocalizationKey>().Property(s => s.Base).IsRequired(false);

            builder.Entity<LocalizationRecord>().Property<Guid>("Culture_Id");
            builder.Entity<LocalizationRecord>().Property<Guid>("LocKey_Id");
            builder.Entity<LocalizationRecord>().HasKey("LocKey_Id", "Culture_Id");
            builder.Entity<LocalizationRecord>().Property<DateTime>(UPDATED_TIMESTAMP);
            builder.Entity<LocalizationRecord>().HasOne(l => l.Culture)
                .WithMany(c => c.Records)
                .IsRequired()
                .HasForeignKey("Culture_Id");
            builder.Entity<LocalizationRecord>().HasOne(l => l.LocalizationKey)
                .WithMany(l => l.Records)
                .IsRequired()
                .HasForeignKey("LocKey_Id");

            base.OnModelCreating(builder);
        }

        /// <summary>
        /// Get all <see cref="LocalizationKey"/> that do no have a localization in a culture.
        /// </summary>
        /// <param name="culture">The culture</param>
        /// <returns>All keys with missing localizations</returns>
        public IQueryable<LocalizationKey> GetMissingLocalizationsForCulture(SupportedCulture culture)
            => LocalizationKeys
                .Where(k =>!SupportedCultures
                    .Where(c => c.Id == culture.Id)
                    .SelectMany(s => s.Records
                        .Where(r => r.Status != RecordStatus.New)
                        .Select(r => r.LocalizationKey.Id)
                    )
                    .Contains(k.Id)
                );

        /// <inheritdoc />
        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            _setUpdatedTimeStampForLocalizationRecord();
            return base.SaveChanges();
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.DetectChanges();
            _setUpdatedTimeStampForLocalizationRecord();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            _setUpdatedTimeStampForLocalizationRecord();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            _setUpdatedTimeStampForLocalizationRecord();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void _setUpdatedTimeStampForLocalizationRecord()
        {
            var modifiedSourceInfo = ChangeTracker.Entries<LocalizationRecord>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedSourceInfo)
                entry.Property(UPDATED_TIMESTAMP).CurrentValue = DateTime.UtcNow;
        }
    }
}
