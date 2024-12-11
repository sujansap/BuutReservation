using System;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Auth;
using Rise.Shared.Boats;

namespace Rise.Services.Boats
{
    public class BoatService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
         : AuthenticatedService(dbContext, authContextProvider), IBoatService
    {


        /// <summary>
        /// get the count of active boats
        /// </summary>
        /// <returns>the count of active boats </returns>        
        public async Task<int> GetActiveBoatsCountAsync()
        {
            return await _dbContext.Boats
                .CountAsync(b => !b.IsDeleted);
        }

        
    }

}

