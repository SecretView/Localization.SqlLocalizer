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
    public class SimpleControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public SimpleControllerTests(WebApplicationFactory<Startup> fixture)
        {
            fixture = fixture.WithWebHostBuilder(b => b.ConfigureServices(services =>
            {
                services.AddDbContext<Data.LocalizationContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(SimpleControllerTests)));
            }));

            using (var scope = fixture.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<Data.LocalizationContext>();
                DatabaseSeeder.InitializeDatabase(context);
            }

            fixture = fixture.WithWebHostBuilder(b => b.ConfigureServices(services =>
            {
                services.AddGlobalization(optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(SimpleControllerTests)));
            }));

            // Arrange
            _client = fixture.CreateDefaultClient();
            _client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        }

        [Fact]
        public async Task Raw_String()
        {
            // Act
            var response = await _client.GetAsync("/Simple/HelloWorld");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Hello world!", responseString);
        }

        [Fact]
        public async Task Raw_String_With_Argument()
        {
            // Act
            var response = await _client.GetAsync("/Simple/HelloPerson?person=Gerard");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Hello Gerard!", responseString);
        }

        [Fact]
        public async Task Force_Language()
        {
            // Act
            var response = await _client.GetAsync("/Simple/HelloWorld?ui-culture=nl");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Hallo wereld!", responseString);
        }

        [Fact]
        public async Task Fallback_Language()
        {
            // Act
            var response = await _client.GetAsync("/Simple/HelloThing?ui-culture=nl");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Hello something!", responseString);
        }

        [Fact]
        public async Task Force_Language_With_Argument()
        {
            // Act
            var response = await _client.GetAsync("/Simple/HelloPerson?person=Gerard&ui-culture=nl");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Hallo Gerard!", responseString);
        }
    }
}
