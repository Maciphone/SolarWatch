using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SolarWatch.Data;
using SolarWatch.Model;
using SolarWatch.Service;
using SolarWatch.Service.Authentication;
using SolarWatch.Service.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//UserSecret
builder.Configuration.AddUserSecrets<Program>();
AddAuthentication();
AddDbContext();
AddIdentity();
ConfigureSwagger();

builder.Services.AddControllers(
    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddSingleton<ICoordinateProvider, CoordinateProviderOWApi>();
builder.Services.AddSingleton<IJsonProcessor<Coordinate>, CoordinateExtractor>();
builder.Services.AddSingleton<IJsonProcessor<SolarWatch.Model.SolarWatch>, JsonProcessor>();
builder.Services.AddSingleton<IJsonProcessor<City>, CityJsonProcessor>();
builder.Services.AddSingleton<ISunsetProvider, SunsetProvider>();


builder.Services.AddScoped<ICityRepository<SunriseSunset, SunriseSunsetDto>, SunriseSunsetRepository>();
builder.Services.AddScoped<ICityRepository<City, CityDto>, CityRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenValidator, TokenValidator>();
builder.Services.AddScoped<AuthenticationSeeder>();

builder.Services.AddDbContext<ApplicationDbContext>();



var app = builder.Build();


//AuthentiacationSeeder registration
using var scope = app.Services.CreateScope(); // AuthenticationSeeder is a scoped service, therefore we need a scope instance to access it
var authenticationSeeder = scope.ServiceProvider.GetRequiredService<AuthenticationSeeder>();
authenticationSeeder.AddRoles(); //User and Admin roles 
authenticationSeeder.AddAdmin(); // admin created and added

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
//Integration Test 

//Integration Test 



//77aa37127f2df8129c4ee9edf8884112 openweather ky

void AddDbContext()
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("The ConnectionString property has not been initialized.");
    }
    Console.WriteLine($"ConnectionString: {connectionString}"); // Debugging purposes

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer("Server=127.0.0.1,1433;Database=Solarwatch;User Id=sa;Password=Macko1234;Encrypt=false;");
    });
    
    builder.Services.AddDbContext<IdentityUserContext>(options =>
    {
        options.UseSqlServer("Server=127.0.0.1,1433;Database=Solarwatch;User Id=sa;Password=Macko1234;Encrypt=false;");
    });
    
//     Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));
// //builder.Services.AddDbContext<WeatherApiContext>();
//     builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     {
//         options.UseSqlServer(builder.Configuration["DbConnection"]);
//     });
//     
//     builder.Services.AddDbContext<IdentityUserContext>(options =>
//     {
//         options.UseSqlServer(builder.Configuration["DbConnection"]);
//     });
    
    
}

void AddAuthentication()
    {
//Authentication registration  Microsoft.AspNetCore.Authentication.JwtBearer
        var jwtStettings =
            builder.Configuration
                .GetSection("JWTSettings"); //IConfigurationSection object - kulcsokra hivatkozva get value like a json
        var validIssuer = jwtStettings["ValidIssuer"];
        //var sameButDifferentWay = builder.Configuration["JWTSettings:ValidIssuer"];
        var validAudience = jwtStettings["ValidAudience"];
        var issuerSigningKey = builder.Configuration["IssuerSigningKey"];
        
        builder.Services
            //Microsoft.AspNetCore.Authentication.JwtBearer Nuget install
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "apiWithBackend",
                    ValidAudience = "apiWithBackend",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("!SomethingSecret123456789Something!Secret!")
                    ),
                };
            });
    }

void AddIdentity()
{


//https://journey.study/v2/learn/materials/asp-register-2q2023
    builder.Services
        .AddIdentityCore<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>() //Enable Identity roles 
        .AddEntityFrameworkStores<IdentityUserContext>();
}

void ConfigureSwagger()
{

    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
    

}

public partial class Program { }
