using System.Net.Http.Json;
using Rise.Shared;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Microsoft.AspNetCore.WebUtilities;

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

        public async Task<ReservationDetailsDto> GetReservationDetailsAsync(int reservationId)
        {
            var result = await _httpClient.GetFromJsonAsync<ReservationDetailsDto>(reservationId.ToString())
                    ?? throw new Exception($"Failed to get reservation details for ID {reservationId}");

            return result;
        }

        public async Task<ItemsPageDto<ReservationDto>> GetUserReservations(int userId, int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 5)
        {
            Dictionary<string, string?> queries = new()
            {
                ["getPast"] = getPast.ToString(),
                ["pageSize"] = pageSize.ToString(),
            };

            if (cursor is not null)
                queries.Add("cursor", cursor.ToString());

            if (isNextPage is not null)
                queries.Add("isNextPage", isNextPage.ToString());

            string queryString = QueryHelpers.AddQueryString("me", queries);

            ItemsPageDto<ReservationDto> result = await _httpClient.GetFromJsonAsync<ItemsPageDto<ReservationDto>>(queryString)
                ?? new ItemsPageDto<ReservationDto> { Data = [] };

            return result;

        }
    }
}
