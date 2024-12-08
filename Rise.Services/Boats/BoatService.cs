using Rise.Shared.Boats;
using Rise.Services.Auth;
using Rise.Persistence;
using Rise.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Rise.Shared;

namespace Rise.Services.Boats
{
    public class BoatService : AuthenticatedService, IBoatService
    {
        public BoatService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
            : base(dbContext, authContextProvider)
        {
        }

        public async Task<IEnumerable<BoatDto>> GetAllBoatsAsync()
        {


            var boats = await _dbContext.Boats
                .Select(boat => new BoatDto
                {
                    Id = boat.Id,
                    PersonalName = boat.PersonalName,
                    IsAvailable = boat.IsAvailable
                })
                .ToListAsync();

            return boats;
        }
    }
}


