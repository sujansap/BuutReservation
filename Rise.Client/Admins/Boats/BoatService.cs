using System.Net.Http.Json;
using Rise.Shared.Boats;

namespace Rise.Client.Services
{
    public class BoatService : IBoatService
    {
        private readonly HttpClient _httpClient;

        public BoatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetActiveBoatsCountAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<int>("count");
            return result;
        }
    }
}