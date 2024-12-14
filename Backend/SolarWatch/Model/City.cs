namespace SolarWatch.Model;

public class City
{
    public int CityId { get; set; } 
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? State { get; set; }
    public string Country { get; set; }
    public ICollection<SunriseSunset> SunriseSunsets { get; set; }
}
/*
 State lehet null, utólag módosítva string? nullable propertyvel, db-t frissíteni
 lentiek alapján
dotnet ef migrations add AddNullableStateToCity
dotnet ef database update
*/