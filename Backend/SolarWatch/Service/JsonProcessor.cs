using System.Globalization;
using System.Text.Json;

namespace SolarWatch.Service;

public class JsonProcessor : IJsonProcessor<Model.SolarWatch>
{
    public Model.SolarWatch Process(string data)
    {
        JsonDocument json = JsonDocument.Parse(data);
        JsonElement results = json.RootElement.GetProperty("results");
        
        string sunriseStr = results.GetProperty("sunrise").GetString();
        string sunsetStr = results.GetProperty("sunset").GetString();
        Console.WriteLine((sunriseStr));
        Console.WriteLine(sunsetStr);
       

        var sunriseDateTime = DateTime.Parse(sunriseStr);
        var sunsetDateTime = DateTime.Parse(sunsetStr);

       
        TimeOnly sunrise = TimeOnly.FromDateTime(sunriseDateTime);
        TimeOnly sunset = TimeOnly.FromDateTime(sunsetDateTime);

        return new Model.SolarWatch(sunrise, sunset);
        
    }
}