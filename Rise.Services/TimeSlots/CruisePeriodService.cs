using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Domain.TimeSlots;
using Rise.Persistence;
using Rise.Services.Auth;
using Rise.Shared.TimeSlots;

namespace Rise.Services.TimeSlots
{
    public class CruisePeriodService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
        : AuthenticatedService(dbContext, authContextProvider), ICruisePeriodService
    {
        public async Task<CruisePeriodDetailedDto> GetCruisePeriod(int id)
        {
            CruisePeriod cruisePeriod = await _dbContext.CruisePeriods.FirstOrDefaultAsync(cp => cp.Id == id) ?? throw new EntityNotFoundException(nameof(CruisePeriod), id);

            return new CruisePeriodDetailedDto()
            {
                Id = cruisePeriod.Id,
                Start = cruisePeriod.Start,
                End = cruisePeriod.End
            };
        }
    }
}
