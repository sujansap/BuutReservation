using System.Net.Http.Json;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
// FIXME
namespace Rise.Client.Services
{
    public class ReservationService(HttpClient httpClient) : IReservationService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<int> CreateReservation(CreateReservationDto reservationDto)
        {
            var result = await _httpClient.PostAsJsonAsync("", reservationDto);
            return await result.Content.ReadFromJsonAsync<int>();
        }

        public Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ItemsPageDto<ReservationDto>> GetUserReservations(int userId, int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 5)
        {
            var query = new List<string>();
            
            if (cursor.HasValue)
            {
                query.Add($"cursor={cursor.Value}");
            }
            
            if (isNextPage.HasValue)
            {
                query.Add($"isNextPage={isNextPage.Value}");
            }
            
            query.Add($"getPast={getPast}");
            query.Add($"pageSize={pageSize}");

            var queryString = string.Join("&", query);

            var result = await _httpClient.GetFromJsonAsync<ItemsPageDto<ReservationDto>>($"me?{queryString}")
                ?? new ItemsPageDto<ReservationDto> { Data = Enumerable.Empty<ReservationDto>() };

            return result;
        }
    }
}
