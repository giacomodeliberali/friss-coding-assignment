using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Person;
using IntegrationTests.Setup;
using Shouldly;
using Web.Host;
using Xunit;

namespace IntegrationTests
{
    public class PeopleControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private const string AndrewCrawId = "26a02c40-c303-447a-ab58-c0a98c1341ea";
        private const string AndrewCraw2Id = "a35ad06e-33fb-4201-bd38-e3da29445676";
        private const string AndrewCrawWithIdentificationNumberId = "aada3c0c-bbaf-490c-9078-8b8503a5982d";
        private const string ACrawId = "3215bd09-a18b-4f70-b1f8-24ad33c7bc1d";
        private const string PettySmithId = "7c922928-fe5f-417f-9017-d882b23be5ce";
        private const string PettySmithWithIdentificationNumberId = "3677d252-7dc6-4562-b9c4-ab1e19648530";

        private readonly HttpClient _httpClient;

        public PeopleControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Theory]
        [InlineData(AndrewCrawId, AndrewCrawId, 1, 0)]
        [InlineData(AndrewCrawId, AndrewCraw2Id, 0.6, 2)]
        [InlineData(AndrewCrawId, PettySmithId, 0.4, 1)]
        [InlineData(AndrewCrawId, ACrawId, 0.95, 3)]
        [InlineData(AndrewCrawWithIdentificationNumberId, PettySmithWithIdentificationNumberId, 1, 1)]
        public async Task CalculateProbabilityForDemoUser(string first, string second, decimal expected, int contributorsCount)
        {
            var url = $"/api/people/probability-same-identity?firstPersonId={first}&secondPersonId={second}";
            var result = await _httpClient.GetFromJsonAsync<ProbabilitySameIdentityDto>(url);

            result.ShouldNotBeNull();
            result.Probability.ShouldBe(expected);
            result.Contributors.Count.ShouldBe(contributorsCount);
        }

        [Fact]
        public async Task Should_Throw_WhenProvidingInvalidStrategy()
        {
            var url = $"/api/people/probability-same-identity?firstPersonId={AndrewCrawId}&secondPersonId={AndrewCraw2Id}&strategyName=NonExisting";
            var result = await _httpClient.GetAsync(url);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);

            var exception = await result.Content.ReadFromJsonAsync<ExceptionDto>();
            exception.ShouldNotBeNull();
            exception.Name.ShouldContain("StrategyNotFoundException");
        }

        [Theory]
        [InlineData(AndrewCraw2Id, "6c922928-fe5f-417f-9017-d882b23be5cc")]
        [InlineData("6c922928-fe5f-417f-9017-d882b23be5cc", AndrewCraw2Id)]
        public async Task Should_Throw_WhenProvidingInvalidUsers(string user1, string user2)
        {
            var url = $"/api/people/probability-same-identity?firstPersonId={user1}&secondPersonId={user2}";
            var result = await _httpClient.GetAsync(url);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);

            var exception = await result.Content.ReadFromJsonAsync<ExceptionDto>();
            exception.ShouldNotBeNull();
            exception.Name.ShouldContain("PersonNotFoundException");
        }
    }
}
