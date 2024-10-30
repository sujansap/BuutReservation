using System.Net.Http.Json;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
// FIXME
namespace Rise.Client.Services
{
    public class ReservationService(HttpClient httpClient) : IReservationService
    {
        private readonly HttpClient _httpClient = httpClient;

        public Task<ReservationsRangeDto> GetAllReservationsInRangeByCurrentUser(DateOnly startDate, DateOnly endDate, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ItemsPageDto<ReservationDto>> GetUserReservations(int userId, int? cursor, bool? isNextPage, bool getPast = false, int pageSize = 3)
        {
            Console.WriteLine("HERE: GetUserReservations");
            //cursor=1&isNextPage=false&getPast=false&pageSize=1' \
            var query = new List<string>();
            query.Add($"cursor={cursor}");
            query.Add($"isNextPage={isNextPage}");
            query.Add($"getPast={getPast}");
            query.Add($"pageSize={pageSize}");

            var queryString = string.Join("&", query);
            Console.WriteLine("Query: " + queryString);

            var result = await _httpClient.GetFromJsonAsync<ItemsPageDto<ReservationDto>>($"me?{queryString}")
                ?? new ItemsPageDto<ReservationDto> { Data = Enumerable.Empty<ReservationDto>() };


            Console.WriteLine("Result: " + result.Data.Count());
            return result;

        }
    }
}