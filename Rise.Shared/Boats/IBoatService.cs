namespace Rise.Shared;

public interface IBoatService
{
    /// <summary>
    /// Haalt een lijst van alle boten op, inclusief hun beschikbaarheid.
    /// </summary>
    /// <returns>Een lijst van BoatDto-objecten.</returns>
    Task<IEnumerable<BoatDto>> GetAllBoatsAsync();
}
