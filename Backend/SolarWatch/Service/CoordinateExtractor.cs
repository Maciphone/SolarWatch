using System.Text.Json;
using SolarWatch.Model;

namespace SolarWatch.Service;

public class CoordinateExtractor : IJsonProcessor<Coordinate>
{
    private readonly ILogger<CoordinateExtractor> _logger;

    public CoordinateExtractor(ILogger<CoordinateExtractor> logger)
    {
        _logger = logger;
    }

    public Coordinate Process(string response)
    {
   
        var jsonDocument = JsonDocument.Parse(response);
        var root = jsonDocument.RootElement;
        
        if (root.GetArrayLength() == 0)
        {
            _logger.LogWarning("No data returned from API for the given city.");
            throw new Exception("invalid city");
        }

        var firstResult = root[0];
        var latitude = firstResult.GetProperty("lat").GetDouble();
        var longitude = firstResult.GetProperty("lon").GetDouble();

        return new Coordinate
        {
            Lattitude = latitude,
            Longitude = longitude,
            
        };
        
    }
}