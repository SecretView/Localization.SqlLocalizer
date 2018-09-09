// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;

namespace SecretCollect.Localization.SqlLocalizer.Internal
{
    internal class DisableCS1591CSharpMigrationsGenerator : CSharpMigrationsGenerator
    {
        private static string BEGIN_OF_FILE_LICENSE = 
            "// Copyright (c) SecretCollect B.V. All rights reserved." + Environment.NewLine +
            "// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information." + Environment.NewLine +
            Environment.NewLine;
        private const string BEGIN_OF_FILE_PRAGMA = "#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member";
        private const string END_OF_FILE_PRAGMA = "#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member";

        public DisableCS1591CSharpMigrationsGenerator(
            MigrationsCodeGeneratorDependencies dependencies,
            CSharpMigrationsGeneratorDependencies csharpDependencies)
            : base(dependencies, csharpDependencies)
        {
        }

        public override string GenerateMetadata(string migrationNamespace, Type contextType, string migrationName, string migrationId, IModel targetModel)
            => BEGIN_OF_FILE_LICENSE
            + BEGIN_OF_FILE_PRAGMA
            + Environment.NewLine
            + base.GenerateMetadata(migrationNamespace, contextType, migrationName, migrationId, targetModel)
            + END_OF_FILE_PRAGMA;

        public override string GenerateSnapshot(string modelSnapshotNamespace, Type contextType, string modelSnapshotName, IModel model)
            => BEGIN_OF_FILE_LICENSE
            + BEGIN_OF_FILE_PRAGMA
            + Environment.NewLine
            + base.GenerateSnapshot(modelSnapshotNamespace, contextType, modelSnapshotName, model)
            + END_OF_FILE_PRAGMA;


        public override string GenerateMigration(string migrationNamespace, string migrationName, IReadOnlyList<MigrationOperation> upOperations, IReadOnlyList<MigrationOperation> downOperations)
            => BEGIN_OF_FILE_LICENSE
            + BEGIN_OF_FILE_PRAGMA
            + Environment.NewLine
            + base.GenerateMigration(migrationNamespace, migrationName, upOperations, downOperations)
            + END_OF_FILE_PRAGMA;
    }
}
