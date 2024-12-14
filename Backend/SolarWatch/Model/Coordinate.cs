namespace SolarWatch.Model;

public class Coordinate
{
    public double Lattitude { get; init; }
    public double Longitude { get; init; }
    
    public string City { get; init; }

    public Coordinate(double lattitude, double longitude, string city)
    {
        Lattitude = lattitude;
        Longitude = longitude;
        City = city;
    }

    public Coordinate()
    {
    }
}