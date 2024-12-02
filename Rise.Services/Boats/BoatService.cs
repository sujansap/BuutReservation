using Rise.Shared.Boats;
using Rise.Services.Auth;
using Rise.Persistence;
using Rise.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Exceptions;

namespace Rise.Services.Boats
{
    public class BatteryService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
        : AuthenticationService(dbContext, authContextProvider), IBatteryService
    {
        public async Task<BatteryDto> UpdateBattery(int id, BatteryUpdateDto newBattery)
        {
            Battery? battery = await _dbContext.Batteries.FirstOrDefaultAsync(b => b.Id == id) ?? throw new EntityNotFoundException(nameof(Battery), id);

            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == newBattery.MentorId) ?? throw new EntityNotFoundException(nameof(User), newBattery.MentorId);

            battery.Mentor = user;
            battery.Type = newBattery.Type;

            await _dbContext.SaveChangesAsync();

            return new BatteryDto { MentorId = battery.Mentor.Id, Type = battery.Type };
        }
    }
}
