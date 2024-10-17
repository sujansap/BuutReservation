using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.Products;
using System.Net.Http.Json;

namespace Rise.Server.Tests.Controllers
{
    public class ProductController(ApiWebApplicationFactory fixture) : IntegrationTest(fixture)
    {
        [Fact]
        public async Task GET_Products_GivesProducts()
        {
            List<ProductDto> response = (await _client.GetFromJsonAsync<List<ProductDto>>("Product"))!;
            response.ShouldNotBeEmpty();
        }
    }
}