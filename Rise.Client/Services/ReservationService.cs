using System.Net.Http.Json;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
// FIXME
namespace Rise.Client.Services
{
    public class ReservationService(HttpClient httpClient) : IReservationService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<ItemsPageDto<ReservationDto>> GetUserReservations(int userId, int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 3)
        {
            return null;
            // return await _httpClient.GetFromJsonAsync<IEnumerable<ReservationDto>>($"user/{userId}?getPast={getPast}");
        }
    }
}