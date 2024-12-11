using System.Net.Http.Json;
using Rise.Shared.Boats;

namespace Rise.Client.Admins.Battery
{
    public class BatteryService(HttpClient httpClient) : IBatteryService
    {
        private readonly HttpClient _httpClient = httpClient;

        public Task<IEnumerable<BatteryDto>> GetBatteriesByBoat(int boatId)
        {
            throw new NotImplementedException();
        }

        public async Task<BatteryDto> GetBattery(int id)
        {
            var response = await _httpClient.GetAsync(id.ToString());

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch battery with ID {id}. Response: {response.ReasonPhrase}");
            }

            return (await response.Content.ReadFromJsonAsync<BatteryDto>())!;
        }

        public async Task<BatteryDto> UpdateBattery(int id, BatteryUpdateDto newBattery)
        {
            var response = await _httpClient.PutAsJsonAsync(id.ToString(), newBattery);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update battery with ID {id}. Response: {response.ReasonPhrase}");
            }

            return (await response.Content.ReadFromJsonAsync<BatteryDto>())!;
        }
    }

}

