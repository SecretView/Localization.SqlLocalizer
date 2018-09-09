// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Globalization;
using Xunit;

namespace SecretCollect.Localization.SqlLocalizer.UnitTests
{
    public class SqlStringLocalizerTests
    {
        [Fact]
        public void Get_Same_Value_When_Not_Found()
        {
            // Arrange
            var stringProviderMock = new Mock<IStringProvider>();
            stringProviderMock.Setup(p => p.GetString(It.IsAny<string>(), It.IsAny<CultureInfo>())).Returns<string, CultureInfo>((x, y) => null);
            var localizer = new SqlStringLocalizer(stringProviderMock.Object, "Test.Namespace.Foo.Bar", NullLogger.Instance);

            // Act
            var localizedString = localizer["INPUT_STRING"];

            // Assert
            Assert.Equal("INPUT_STRING", localizedString.Value);
        }

        [Fact]
        public void ResourceNotFound_Is_True_When_Resource_Not_Exists()
        {
            // Arrange
            var stringProviderMock = new Mock<IStringProvider>();
            stringProviderMock.Setup(p => p.GetString(It.IsAny<string>(), It.IsAny<CultureInfo>())).Returns<string, CultureInfo>((x, y) => null);
            var localizer = new SqlStringLocalizer(stringProviderMock.Object, "Test.Namespace.Foo.Bar", NullLogger.Instance);

            // Act
            var localizedString = localizer["INPUT_STRING"];

            // Assert
            Assert.True(localizedString.ResourceNotFound);
        }

        [Fact]
        public void ResourceNotFound_Is_False_When_Resource_Exists()
        {
            // Arrange
            var stringProviderMock = new Mock<IStringProvider>();
            stringProviderMock.Setup(p => p.GetString(It.IsAny<string>(), It.IsAny<CultureInfo>())).Returns<string, CultureInfo>((x, y) => "Invoer tekst");
            var localizer = new SqlStringLocalizer(stringProviderMock.Object, "Test.Namespace.Foo.Bar", NullLogger.Instance);

            // Act
            var localizedString = localizer["INPUT_STRING"];

            // Assert
            Assert.False(localizedString.ResourceNotFound);
        }

        [Fact]
        public void Return_Correct_Localization_Based_On_Ui_Culture()
        {
            // Arrange
            const string lookupKey = "INPUT_STRING";
            const string nlAnswer = "Invoer tekst";
            const string enAnswer = "Input text";
            var stringProviderMock = new Mock<IStringProvider>();
            stringProviderMock.Setup(p => p.GetString(lookupKey, CultureInfo.GetCultureInfo("nl"))).Returns<string, CultureInfo>((x, y) => nlAnswer);
            stringProviderMock.Setup(p => p.GetString(lookupKey, CultureInfo.GetCultureInfo("en"))).Returns<string, CultureInfo>((x, y) => enAnswer);
            var localizer = new SqlStringLocalizer(stringProviderMock.Object, "Test.Namespace.Foo.Bar", NullLogger.Instance);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");

            // Act
            var localizedString = localizer[lookupKey];

            // Assert
            Assert.Equal(nlAnswer, localizedString.Value);
            stringProviderMock.Verify(moq => moq.GetString(lookupKey, CultureInfo.GetCultureInfo("nl")), Times.AtLeastOnce());
            stringProviderMock.Verify(moq => moq.GetString(lookupKey, CultureInfo.GetCultureInfo("en")), Times.Never());
        }

        [Fact]
        public void With_Culture_Returns_New_Localizer_With_Selected_Culture()
        {
            // Arrange
            const string lookupKey = "INPUT_STRING";
            const string nlAnswer = "Invoer tekst";
            const string enAnswer = "Input text";
            var stringProviderMock = new Mock<IStringProvider>();
            stringProviderMock.Setup(p => p.GetString(lookupKey, CultureInfo.GetCultureInfo("nl"))).Returns<string, CultureInfo>((x, y) => nlAnswer);
            stringProviderMock.Setup(p => p.GetString(lookupKey, CultureInfo.GetCultureInfo("en"))).Returns<string, CultureInfo>((x, y) => enAnswer);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
            var currentUiLocalizer = new SqlStringLocalizer(stringProviderMock.Object, "Test.Namespace.Foo.Bar", NullLogger.Instance);

            // Act
            var englishLocalizer = currentUiLocalizer.WithCulture(CultureInfo.GetCultureInfo("en"));
            var localizedString = englishLocalizer[lookupKey];

            // Assert
            Assert.Equal(enAnswer, localizedString.Value);
        }
    }
}
