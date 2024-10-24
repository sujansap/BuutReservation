using System.Net.Http.Json;
using Rise.Shared.Reservations;

namespace Rise.Client.Services
{
    public class ReservationService(HttpClient httpClient) : IReservationService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<IEnumerable<ReservationListDto>> GetCurrentUserReservations(int userId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ReservationListDto>>($"user/{userId}");
        }
    }
}