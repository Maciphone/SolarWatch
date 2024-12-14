using System.Text.Json;
using SolarWatch.Model;

namespace SolarWatch.Service;

public class CityJsonProcessor : IJsonProcessor<City>
{
    private ILogger<CityJsonProcessor> _logger;

    public CityJsonProcessor(ILogger<CityJsonProcessor> logger)
    {
        _logger = logger;
    }

    public City Process(string response)
    {
        var jsonDocument = JsonDocument.Parse(response);
        var root = jsonDocument.RootElement;
        
        if (root.GetArrayLength() == 0)
        {
            _logger.LogWarning("No data returned from API for the given city.");
            throw new Exception("invalid city");
        }

        var firstResult = root[0];
        var name = firstResult.GetProperty("name").ToString();
        var latitude = firstResult.GetProperty("lat").GetDouble();
        var longitude = firstResult.GetProperty("lon").GetDouble();
       // var state = firstResult.GetProperty("state").ToString();
        var country = firstResult.GetProperty("country").ToString();
        
        string state = firstResult.TryGetProperty("state", out var stateProp) ? stateProp.GetString() : null;


        return new City
        {
            Name = name,
            Country = country,
            Latitude = latitude,
            Longitude = longitude,
            State = state
        };
    }
}