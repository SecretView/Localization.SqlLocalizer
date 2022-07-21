// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using SimpleMvc;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SecretCollect.Localization.SqlLocalizer.IntegrationTests
{
    public class DataAnnotationsTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public DataAnnotationsTests(CustomWebApplicationFactory<Startup> fixture)
        {
            // Arrange
            _client = fixture.CreateDefaultClient();
            _client.DefaultRequestHeaders.Add("Accept-Language", "nl-NL,nl;q=0.9");
        }

        [Fact]
        public async Task Select_Using_Enum_Localizes()
        {
            // Act
            var response = await _client.GetAsync("/View/SelectListUsingEnum/");

            var responseString = await response.Content.ReadAsStringAsync();
            var xElement = System.Xml.Linq.XElement.Parse(responseString);

            // Assert
            Assert.Equal("Januari", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "1").Value);
            Assert.Equal("Februari", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "2").Value);
            Assert.Equal("Maart", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "3").Value);
            Assert.Equal("April", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "4").Value);
            Assert.Equal("Mei", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "5").Value);
            Assert.Equal("Juni", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "6").Value);
            Assert.Equal("Juli", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "7").Value);
            Assert.Equal("Augustus", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "8").Value);
            Assert.Equal("September", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "9").Value);
            Assert.Equal("Oktober", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "10").Value);
            Assert.Equal("November", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "11").Value);
            Assert.Equal("December", xElement.Elements("option").SingleOrDefault(o => o.Attribute("value")?.Value == "12").Value);
        }

        [Fact]
        public async Task Select_Using_Enum_Keeps_Working_After_Multiple_Requests()
        {
            // Act
            var response1 = await _client.GetAsync("/View/SelectListUsingEnum/");
            var response2 = await _client.GetAsync("/View/SelectListUsingEnum/");

            // Assert
            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.IsSuccessStatusCode);
        }
    }
}
