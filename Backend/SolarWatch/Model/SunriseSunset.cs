namespace SolarWatch.Model;

public class SunriseSunset
{
    public int SunriseSunsetId { get; set; }
    public DateTime Sunrise { get; set; }
    public DateTime Sunset { get; set; }
    public int CityId { get; set; } 
    public City City { get; set; }
}