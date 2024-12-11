using System;

namespace Rise.Shared.Boats;

public interface IBoatService
{
    Task<int> GetActiveBoatsCountAsync();

}
