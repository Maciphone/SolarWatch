using Microsoft.EntityFrameworkCore;
using SolarWatch.Model;

namespace SolarWatch.Data;

public class ApplicationDbContext : DbContext
{
    
    private readonly IConfiguration _configuration;

    // public ApplicationDbContext(IConfiguration configuration)
    // {
    //     _configuration = configuration;
    // }
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DbConnection"));
    //     base.OnConfiguring(optionsBuilder);
    // program.cs > AddDbContext()
    // }

    //INTEGRATION test miatt <Application> berakva, előtte nem volt
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; }
    public DbSet<SunriseSunset> SunriseSunsets { get; set; }
    
    
    /*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=127.0.0.1,1433;Database=Solarwatch;User Id=sa;Password=Macko1234;Encrypt=false;");
    }
*/
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>()
            .HasMany(c => c.SunriseSunsets)
            .WithOne(s => s.City)
            .HasForeignKey(s => s.CityId);
    }
}

/*
  dotnet tool install --global dotnet-ef --version 7.0.14
     dotnet add package Microsoft.EntityFrameworkCore.Design -v 7.0.14
     dotnet ef migrations add InitialCreate
     dotnet ef database update
     */