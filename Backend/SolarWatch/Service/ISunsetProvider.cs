using SolarWatch.Model;

namespace SolarWatch.Service;

public interface ISunsetProvider
{
    string GetSunset(Coordinate coordinate, DateTime date);
    Task<string> GetSunsetAsync(Coordinate coordinate, DateTime date);

}