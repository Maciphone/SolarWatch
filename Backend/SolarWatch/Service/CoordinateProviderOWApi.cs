using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SolarWatch.Model;

namespace SolarWatch.Service;

public class CoordinateProviderOWApi : ICoordinateProvider
{
    private readonly ILogger<CoordinateProviderOWApi> _logger;
    private readonly IJsonProcessor<Coordinate> _coordinateExtractor;

    public CoordinateProviderOWApi(ILogger<CoordinateProviderOWApi> logger,
        IJsonProcessor<Coordinate> coordinateExtractor)
    {
        _logger = logger;
        _coordinateExtractor = coordinateExtractor;
    }


    public Coordinate GetCoordinates(string city)
    {
        var apiKey = "f778716bc3508229848e8235af6fca4e";
        var url = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=5&appid={apiKey}";
     
            using var client = new WebClient();

            _logger.LogInformation("Calling OpenWeather API with url: {url}", url);
            var response = client.DownloadString(url);
            Console.WriteLine($"response cityapi: {response}");
            if (string.IsNullOrEmpty(response))
            {
                return null;
            }
            else
            {
                var result = _coordinateExtractor.Process(response);
                return result;
            }

    }

    public async Task<string> GetAsyncCoordinate(string city)
    {
        var apiKey = "f778716bc3508229848e8235af6fca4e";
        var url = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=5&appid={apiKey}";

        http://api.openweathermap.org/geo/1.0/direct?q=London&limit=5&appid=f778716bc3508229848e8235af6fca4e
        using var client = new HttpClient();
        _logger.LogInformation("Calling OpenWeather API for coordinates with url: {url}", url);

        var response = await client.GetAsync(url);

        var stringRespone = await response.Content.ReadAsStringAsync();
        return stringRespone;

    }
}
