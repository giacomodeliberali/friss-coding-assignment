using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Contracts.Rules;
using Application.Rules;
using Domain.Extensions;
using IntegrationTests.Setup;
using Shouldly;
using Web.Host;
using Xunit;

namespace IntegrationTests
{
    public class StrategyControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public StrategyControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Should_ReadAllRegisteredMatchingRules()
        {
            // Arrange
            var url = $"/api/strategies/available-rules";

            // Act
            var result = await _httpClient.GetFromJsonAsync<List<RuleDto>>(url);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(4);
            var rules = result.Select(r => r.AssemblyQualifiedName).ToList();
            rules.Contains(typeof(FirstNameMatchingRule).GetAssemblyQualifiedName()).ShouldBe(true);
            rules.Contains(typeof(LastNameMatchingRule).GetAssemblyQualifiedName()).ShouldBe(true);
            rules.Contains(typeof(BirthDateEqualsMatchingRule).GetAssemblyQualifiedName()).ShouldBe(true);
            rules.Contains(typeof(IdentificationNumberEqualsMatchingRule).GetAssemblyQualifiedName()).ShouldBe(true);
        }
    }
}
