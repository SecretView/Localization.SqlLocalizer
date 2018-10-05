// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.AspNetCore.Mvc.Testing;
using SimpleMvc;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SecretCollect.Localization.SqlLocalizer.IntegrationTests
{
    public class MultipleControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public MultipleControllerTests(WebApplicationFactory<Startup> fixture)
        {
            // Arrange
            _client = fixture.CreateDefaultClient();
            _client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        }

        [Fact]
        public async Task Different_Requests_Keeps_Working_After_One_After_Other()
        {
            // Act
            var response1 = await _client.GetAsync("/Simple/HelloWorld");
            var response2 = await _client.GetAsync("/Another/FooBar");

            // Assert
            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.IsSuccessStatusCode);
        }
    }
}
