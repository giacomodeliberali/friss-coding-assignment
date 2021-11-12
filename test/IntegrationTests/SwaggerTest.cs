using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IntegrationTests.Setup;
using Shouldly;
using Web.Host;
using Xunit;

namespace IntegrationTests
{
    public class SwaggerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public SwaggerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task ShouldReturn_SwaggerDocument()
        {
            var response = await _httpClient.GetAsync("/swagger/v1/swagger.json");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

    }
}
