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
            rules.Contains(typeof(FirstNameMatchingMatchingRule).GetAssemblyQualifiedName()).ShouldBe(true);
            rules.Contains(typeof(LastNameMatchingMatchingRule).GetAssemblyQualifiedName()).ShouldBe(true);
            rules.Contains(typeof(BirthDateEqualsMatchingMatchingRule).GetAssemblyQualifiedName()).ShouldBe(true);
            rules.Contains(typeof(IdentificationNumberEqualsMatchingMatchingRule).GetAssemblyQualifiedName()).ShouldBe(true);
        }

        [Fact]
        public async Task ShouldCreateAStrategy()
        {
            // Arrange
            var url = $"/api/strategies";
            var createStrategyDto = new CreateStrategyDto()
            {
                Name = "Test strategy",
                Description = "My test strategy description",
                Rules = new List<CreateStrategyDto.CreateRuleDto>()
                {
                    new()
                    {
                        Name = "Test firstname match rule",
                        Description = "When first name are similar or equal",
                        IsEnabled = true,
                        RuleTypeAssemblyQualifiedName = "Application.Rules.FirstNameMatchingMatchingRule, Application",
                    }
                }
            };

            // Act
            var result = await _httpClient.PostAsJsonAsync(url, createStrategyDto);

            // Assert
            result.ShouldNotBeNull();
            result.EnsureSuccessStatusCode();
            var createdStrategy = await result.Content.ReadFromJsonAsync<CreateStrategyReplyDto>();
            createdStrategy.ShouldNotBeNull();
        }
    }
}
