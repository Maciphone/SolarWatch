using System.Globalization;
using System.Net;
using SolarWatch.Model;

namespace SolarWatch.Service;

public class 
    SunsetProvider : ISunsetProvider
{
    private readonly ILogger<SunsetProvider> _logger;
    private ICoordinateProvider _openWeatherApi;


    public SunsetProvider(ILogger<SunsetProvider> logger, ICoordinateProvider openWeatherApi)
    {
        _logger = logger;
        _openWeatherApi = openWeatherApi;
    }


    public string GetSunset(Coordinate coordinate, DateTime date)
    {

        var c = coordinate;
        string lat = c.Lattitude.ToString(CultureInfo.InvariantCulture);
        string lon = c.Longitude.ToString(CultureInfo.InvariantCulture);


        Console.WriteLine(c.Lattitude);
        Console.WriteLine(date);

        var url = $"https://api.sunrise-sunset.org/json?lat={lat}&lng={lon}&date={date.Year}-{date.Month}-{date.Day}";

        using var client = new WebClient();
        _logger.LogInformation($"calling sunset api with url {url}");
        return client.DownloadString(url);
    }

    public async Task<string> GetSunsetAsync(Coordinate coordinate, DateTime date)
    {
        var c = coordinate;
        string lat = c.Lattitude.ToString(CultureInfo.InvariantCulture);
        string lon = c.Longitude.ToString(CultureInfo.InvariantCulture);


        Console.WriteLine(c.Lattitude);
        Console.WriteLine(date);

        var url = $"https://api.sunrise-sunset.org/json?lat={lat}&lng={lon}&date={date.Year}-{date.Month}-{date.Day}";

        using var client = new HttpClient();
        var response = await client.GetAsync(url);

        return await response.Content.ReadAsStringAsync();


    }
}
    
    
