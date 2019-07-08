using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using cats_api;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace cats_api_test
{
    public class HttpPipeLineIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {

        private readonly WebApplicationFactory<Startup> _factory;

        public HttpPipeLineIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/cats")]
        public async Task LookForCorrelationIDInResponse(string url)
        {
            // Arrange
            using (var client = _factory.CreateClient())
            {
                // Act
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Status Code 200-299
                // Assert
                Assert.True(response.Headers.Contains("X-Correlation-ID"), "Did not find CorrelationID header in Response");
            }
        }

         [Theory]
        [InlineData("/cats/4")]
        public async Task LookForCorrelationIDInProblemDetailResponse(string url)
        {
            // Arrange
            using (var client = _factory.CreateClient())
            {
                // Act
                var response = await client.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();
                var problemDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemDetails>(responseBody);
                // Assert
                Assert.Equal("An unexpected error has occurred!", problemDetails.Title);
                Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode); // Status Code 500                
                Assert.True(response.Headers.Contains("X-Correlation-ID"), "Did not find CorrelationID header in Response");
            }
        }
    }
}
