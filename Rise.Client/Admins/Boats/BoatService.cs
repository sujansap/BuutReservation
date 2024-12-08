using System.Net.Http.Json;
using Rise.Shared;
using Rise.Shared.Boats;

namespace Rise.Client.Services
{
    public class BoatService(HttpClient httpClient) : IBoatService
    {
        private readonly HttpClient _httpClient = httpClient;

        /// <summary>
        /// Haalt alle boten op.
        /// </summary>
        /// <returns>Een lijst van boten.</returns>
        public async Task<IEnumerable<BoatDto>> GetAllBoatsAsync()
        {
            var response = await _httpClient.GetAsync("");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch boats. Response: {response.ReasonPhrase}");
            }

            return (await response.Content.ReadFromJsonAsync<List<BoatDto>>())!;
        }

    }
}
