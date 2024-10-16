using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Respawn;

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

        public IntegrationTest(ApiWebApplicationFactory fixture)
        {
            _factory = fixture;
            // TODO base address for HTTPclient in integrations tests is hardcoded
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions() { BaseAddress = new Uri("https://localhost:5001/api/") });
            // TODO set up respawn for avoiding changes during tests https://github.com/jbogard/respawn
        }
    }
}
