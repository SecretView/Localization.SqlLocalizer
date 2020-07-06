// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            fixture = fixture.WithWebHostBuilder(b => b.ConfigureServices(services =>
            {
                services.AddDbContext<Data.LocalizationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(MultipleControllerTests)));
            }));

            using (var scope = fixture.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<Data.LocalizationContext>();
                DatabaseSeeder.InitializeDatabase(context);
            }

            fixture = fixture.WithWebHostBuilder(b => b.ConfigureServices(services =>
            {
                services.AddGlobalization(optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(MultipleControllerTests)));
            }));

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
