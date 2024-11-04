using System.Net.Http.Json;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Microsoft.AspNetCore.WebUtilities;

namespace Rise.Client.Services
{
    public class ReservationService(HttpClient httpClient) : IReservationService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<ReservationDto> CreateReservation(int timeSlotId)
        {
            var result = await _httpClient.PostAsJsonAsync("", new { timeSlotId });
            return await result.Content.ReadFromJsonAsync<ReservationDto>() ?? new ReservationDto();
        }

        public Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ItemsPageDto<ReservationDto>> GetUserReservations(int userId, int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 5)
        {
            Dictionary<string, string?> queries = new()
            {
                ["cursor"] = cursor.ToString(),
                ["isNextPage"] = isNextPage.ToString(),
                ["getPast"] = getPast.ToString(),
                ["pageSize"] = pageSize.ToString(),
            };

            string queryString = QueryHelpers.AddQueryString("me", queries);

            ItemsPageDto<ReservationDto> result = await _httpClient.GetFromJsonAsync<ItemsPageDto<ReservationDto>>(queryString)
                ?? new ItemsPageDto<ReservationDto> { Data = [] };

            return result;

        }
    }
}
