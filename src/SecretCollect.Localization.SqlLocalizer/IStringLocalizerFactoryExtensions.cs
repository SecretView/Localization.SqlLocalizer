// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using SecretCollect.Localization.SqlLocalizer;
using System;

namespace Microsoft.Extensions.Localization
{
    public static class IStringLocalizerFactoryExtensions
    {
        public static IStringLocalizer Create(this IStringLocalizerFactory factory, string fullKey)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            _ = fullKey ?? throw new ArgumentNullException(nameof(fullKey));

            if(factory is SqlStringLocalizerFactory sqlFactory)
                return sqlFactory.CreateSqlStringLocalizer(fullKey);

            var lastDotPosition = fullKey.LastIndexOf('.');

            var baseName = fullKey.Substring(lastDotPosition);
            var location = fullKey.Substring(0, lastDotPosition).TrimEnd('.');

            return factory.Create(baseName, location);
        }
    }
}
