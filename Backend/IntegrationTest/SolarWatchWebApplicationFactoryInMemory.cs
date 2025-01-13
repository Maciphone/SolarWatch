using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using SolarWatch.Contracts;
using SolarWatch.Data;
using SolarWatch.Model;
using SolarWatch.Service.Authentication;

namespace IntegrationTest;

public class SolarWatchWebApplicationFactoryInMemory : WebApplicationFactory<Program>
{

    //Create a new db name for each SolarWatchWebApplicationFactory. This is to prevent tests failing from changes done in db by a previous test. 
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //Get the previous DbContextOptions registrations 
            var solarWatchDbContextDescriptor = services.SingleOrDefault(d
                => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            var usersDbContextDescriptor = services.SingleOrDefault(d
                => d.ServiceType == typeof(DbContextOptions<IdentityUserContext>));

            //Remove the previous DbContextOptions registrations
            services.Remove(solarWatchDbContextDescriptor);
            services.Remove(usersDbContextDescriptor);

            //Add new DbContextOptions for our two contexts, this time with inmemory db 
            services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase(_dbName); });

            services.AddDbContext<IdentityUserContext>(options => { options.UseInMemoryDatabase(_dbName); });

            //We will need to initialize our in memory databases. 
            //Since DbContexts are scoped services, we create a scope
            using var scope = services.BuildServiceProvider().CreateScope();

            //We use this scope to request the registered dbcontexts, and initialize the schemas
            var solarContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userContext = scope.ServiceProvider.GetRequiredService<IdentityUserContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();



            solarContext.Database.EnsureDeleted();
            solarContext.Database.EnsureCreated();
            //initial inmemory database ApplicationDb
          //  SeedSolarWatchDatabase(solarContext);
                //.GetAwaiter().GetResult();

            userContext.Database.EnsureDeleted();
            userContext.Database.EnsureCreated();
            //initial inmemory userContext
            SeeduserConetext(userContext, userManager, roleManager)
                .GetAwaiter().GetResult();
            //SeeduserConetext(userContext);

            //Here we could do more initializing if we wished (e.g. adding admin user)
            var testCity = new City
            {
            
                Name = "TestCity",
                Latitude = 47.0,
                Longitude = 17.0,
                Country = "TestCountry",
                State = "TestState"
            };
            solarContext.Cities.Add(testCity);
            solarContext.SaveChanges();
            // await solarContext.Cities.AddAsync(testCity);
            //  await solarContext.SaveChangesAsync();


            var sunset = new SunriseSunset
            {
                CityId = testCity.CityId,
                Sunrise = new DateTime(2024, 11, 11, 11, 11, 11),
                Sunset = new DateTime(2024, 11, 11, 11, 12, 11)
                //City = testCity
            };
            solarContext.SunriseSunsets.Add(sunset);
            solarContext.SaveChanges();
            
        });
    }




    private async Task SeeduserConetext(IdentityUserContext userContext, UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var user = new IdentityUser { UserName = "test", Email = "a@a.hu" };
        await userManager.CreateAsync(user, "password");
        await userManager.AddToRoleAsync(user, "User"); // Adding the user to a role
    }

    private void SeedSolarWatchDatabase(ApplicationDbContext solarContext)
    {
        var testCity = new City
        {
            
            Name = "TestCity",
            Latitude = 47.0,
            Longitude = 17.0,
            Country = "TestCountry",
            State = "TestState"
        };
       solarContext.Cities.Add(testCity);
       solarContext.SaveChanges();
      // await solarContext.Cities.AddAsync(testCity);
     //  await solarContext.SaveChangesAsync();


       var sunset = new SunriseSunset
       {
           CityId = testCity.CityId,
           Sunrise = new DateTime(2024, 11, 11, 11, 11, 11),
           Sunset = new DateTime(2024, 11, 11, 11, 12, 11)
           //City = testCity
       };
       solarContext.SunriseSunsets.Add(sunset);
       solarContext.SaveChanges();
       // await solarContext.SunriseSunsets.AddAsync(sunset);
       // await solarContext.SaveChangesAsync();
       



    }
}

/*
        var testUser = new IdentityUser
        {
            UserName = "testuser",
            NormalizedUserName = "TESTUSER",
            Email = "a@a.com",
            NormalizedEmail = "A@A.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
        };

        var passwordHasher = new PasswordHasher<IdentityUser>();
        testUser.PasswordHash = passwordHasher.HashPassword(testUser, "testpassword");

        if (userManager.Users.All(u => u.Id != testUser.Id))
        {
            var result = await userManager.CreateAsync(testUser);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(testUser, "User");
            }
        }

        Console.WriteLine("Seeded user:");
        Console.WriteLine($"Email: {testUser.Email}");
        Console.WriteLine($"Password: testpassword");
    }
    /*
    private void SeeduserConetext(IdentityUserContext userContext)
    {
        var testUser = new IdentityUser
        {
            UserName = "testuser",
            NormalizedUserName = "TESTUSER",
            Email = "a@a.com",
            NormalizedEmail = "A@A.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
        };

        var passwordHasher = new PasswordHasher<IdentityUser>();
        testUser.PasswordHash = passwordHasher.HashPassword(testUser, "testpassword");
        userContext.Users.Add(testUser);
        userContext.SaveChanges();
        Console.WriteLine("Seeded user:");
        Console.WriteLine($"Email: {testUser.Email}");
        Console.WriteLine($"Password: testpassword");
    }

    private void SeedSolarWatchDatabase(ApplicationDbContext solarContext)
    {
       var testCity = new City
        {
            Name = "TestCity",
            Latitude = 47.0,
            Longitude = 17.0,
            Country = "TestCountry",
            State = "TestState"
        };
        solarContext.Cities.Add(testCity);

        solarContext.SunriseSunsets.Add(new SunriseSunset
        {
            CityId = testCity.CityId,
            Sunrise = DateTime.Parse("2024-07-29T05:00:00"),
            Sunset = DateTime.Parse("2024-07-29T20:00:00"),
            City = testCity
        });

        solarContext.SaveChanges();
    }
    */
    

