namespace SolarWatch.Model;

public class SolarWatch
{
    public DateOnly Year { get; set; }
    public TimeOnly Sunrise { get; init; }
    public TimeOnly Sunset { get; init; }
    public string City { get; set; }
    
    //módosítás idk hozzáadása
    public int CityId { get; set; }
    public int SunriseSunsetId { get; set; }

    public SolarWatch(TimeOnly sunrise, TimeOnly sunset, string city)
    {
        Sunrise = sunrise;//hh:mm::ss
        Sunset = sunset;
        City = city;
    }

    public SolarWatch(TimeOnly sunrise, TimeOnly sunset)
    {
        Sunrise = sunrise;
        Sunset = sunset;
    }

    public SolarWatch()
    {
    }
    public SolarWatch WithCity(string city, DateOnly year)
    {
        return new SolarWatch
        {
            Sunrise = this.Sunrise,
            Sunset = this.Sunset,
            City = city,
            Year = year
        };
    }
    
    public override string ToString()
    {
        return $"Sunrise: {Sunrise:HH:mm:ss}, Sunset: {Sunset:HH:mm:ss}, City: {City}";
    }


}