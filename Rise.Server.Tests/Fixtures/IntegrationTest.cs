
namespace Rise.Server.Tests.Fixtures
{
    [Trait("Category", "Integration")]
    public abstract class IntegrationTest : IClassFixture<ApiWebApplicationFactory>
    {

        // private readonly Checkpoint _checkpoint = new()
        // {
        //     SchemasToInclude = [
        //         "Boat",
        //         "CruisePeriods",
        //         "Reservation",
        //         "ReservationUser",
        //         "TimeSlots",
        //         "User",
        // ],
        //     DbAdapter = DbAdapter.Postgres,
        //     WithReseed = true
        // };

        protected readonly ApiWebApplicationFactory _factory;
        protected readonly HttpClient _client;

        public IntegrationTest(ApiWebApplicationFactory fixture, string routeBase)
        {
            _factory = fixture;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri(_client.BaseAddress ?? new Uri("https://localhost"), "api/" + routeBase + "/");
            // TODO set up respawn for avoiding changes during tests https://github.com/jbogard/respawn
        }
    }
}
