using SolarWatch.Model;

namespace SolarWatch.Service;

public interface ICoordinateProvider
{
    Coordinate GetCoordinates(string City);
    Task<string> GetAsyncCoordinate(string City);

}