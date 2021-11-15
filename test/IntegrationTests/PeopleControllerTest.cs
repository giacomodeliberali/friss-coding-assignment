using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Person;
using Application.Seed;
using IntegrationTests.Setup;
using Shouldly;
using Web.Host;
using Xunit;

namespace IntegrationTests
{
    public class PeopleControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public PeopleControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Theory]
        [InlineData(DemoUsersSeed.AndrewCrawId, DemoUsersSeed.AndrewCrawId, 1, 0)]
        [InlineData(DemoUsersSeed.AndrewCrawId, DemoUsersSeed.AndrewCraw2Id, 0.6, 2)]
        [InlineData(DemoUsersSeed.AndrewCrawId, DemoUsersSeed.PettySmithId, 0.4, 1)]
        [InlineData(DemoUsersSeed.AndrewCrawId, DemoUsersSeed.ACrawId, 0.95, 3)]
        [InlineData(DemoUsersSeed.AndrewCrawWithIdentificationNumberId, DemoUsersSeed.PettySmithWithIdentificationNumberId, 1, 1)]
        public async Task CalculateProbabilityForDemoPeople(string first, string second, decimal expected, int contributorsCount)
        {
            // Arrange
            var strategyId = await GetStrategyId();
            var url = $"/api/people/probability-same-identity?firstPersonId={first}&secondPersonId={second}&strategyId={strategyId}";

            // Act
            var result = await _httpClient.GetFromJsonAsync<ProbabilitySameIdentityDto>(url);

            // Assert
            result.ShouldNotBeNull();
            result.Probability.ShouldBe(expected);
            result.Contributors.Count.ShouldBe(contributorsCount);
        }

        [Fact]
        public async Task Should_Throw_WhenProvidingInvalidStrategy()
        {
            // Arrange
            var url = $"/api/people/probability-same-identity?firstPersonId={DemoUsersSeed.AndrewCrawId}&secondPersonId={DemoUsersSeed.AndrewCraw2Id}&strategyId={Guid.NewGuid()}";

            // Act
            var result = await _httpClient.GetAsync(url);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(DemoUsersSeed.AndrewCraw2Id, "6c922928-fe5f-417f-9017-d882b23be5cc")]
        [InlineData("6c922928-fe5f-417f-9017-d882b23be5cc", DemoUsersSeed.AndrewCraw2Id)]
        public async Task Should_Throw_WhenProvidingInvalidUsers(string user1, string user2)
        {
            // Arrange
            var strategyId = await GetStrategyId();
            var url = $"/api/people/probability-same-identity?firstPersonId={user1}&secondPersonId={user2}&strategyId={strategyId}";

            // Act
            var result = await _httpClient.GetAsync(url);

            // Assert
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        private async Task<Guid> GetStrategyId()
        {
            var url = $"/api/strategies";
            var result = await _httpClient.GetFromJsonAsync<List<StrategyDto>>(url);

            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            var strategyId = result.Single().Id;
            strategyId.ShouldNotBe(Guid.Empty);

            return strategyId;
        }
    }
}
