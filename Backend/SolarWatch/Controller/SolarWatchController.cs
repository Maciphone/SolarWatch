using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;
using SolarWatch.Service;
using SolarWatch.Service.Authentication;
using SolarWatch.Service.Repository;

namespace SolarWatch.Controller;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class SolarWatchController : ControllerBase
{
    private readonly ICoordinateProvider _coordinateProvider;
    private readonly ILogger<SolarWatchController> _logger;
    private ISunsetProvider _sunsetProviderProvider;
    private IJsonProcessor<Model.SolarWatch> _jsonProcessor;
    private IJsonProcessor<Coordinate> _jsonProcessorCoordinate;
    private IJsonProcessor<City> _jsonProcessorCity;
    private ICityRepository<City, CityDto> _cityRepository;
    private ICityRepository<SunriseSunset, SunriseSunsetDto> _sunriseSunsetRepository;
    //private ITokenValidator _tokenValidator;

    public SolarWatchController(ILogger<SolarWatchController> logger, ISunsetProvider sunsetProviderProvider,
        IJsonProcessor<Model.SolarWatch> jsonProcessor, ICoordinateProvider coordinateProvider,
        IJsonProcessor<Coordinate> jsonProcessorCoordinate, ICityRepository<City, CityDto> cityRepository,
        ICityRepository<SunriseSunset, SunriseSunsetDto> sunriseSunsetRepository, IJsonProcessor<City> jsonProcessorCity)//ITokenValidator tokenValidator
    {
        _logger = logger;
        _sunsetProviderProvider = sunsetProviderProvider;
        _jsonProcessor = jsonProcessor;
        _coordinateProvider = coordinateProvider;
        _jsonProcessorCoordinate = jsonProcessorCoordinate;
        _cityRepository = cityRepository;
        _sunriseSunsetRepository = sunriseSunsetRepository;
        _jsonProcessorCity = jsonProcessorCity;
        //_tokenValidator = tokenValidator;
    }

    #region BasicEndpoints

    [HttpGet("GetById")]
    public async Task<City> CityById(int id)
    {
        return await _cityRepository.GetByIdAsync(id);
    }

    [HttpGet]
    [Route("GetByCityDate")]
    public ActionResult<Model.SolarWatch> Get(DateTime? date, [Required] string city)
    {
        Coordinate coordinate = null;
        try
        {
            var a = _coordinateProvider.GetCoordinates(city);
            coordinate = a;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound("User fault: invalid city");
        }

        try
        {
            var dateActual = date == null ? DateTime.Now : date.Value;
            var sunsetDate = _sunsetProviderProvider.GetSunset(coordinate, dateActual);
            var solarWatch = _jsonProcessor.Process(sunsetDate);
            solarWatch.City = city;
            solarWatch.Year = DateOnly.FromDateTime(dateActual);
            solarWatch.City = city;


            return Ok(solarWatch);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting weather data");
            return NotFound("Error getting weather data");
        }
    }

    [HttpGet]
    [Route("GetByCityDateAsync")]
    public async Task<ActionResult<Model.SolarWatch>> GetAsync(DateTime? date, [Required] string city)
    {
        Coordinate coordinate = null;
        try
        {
            var coordinateString = await _coordinateProvider.GetAsyncCoordinate(city);
            var responseCoordinate = _jsonProcessorCoordinate.Process(coordinateString);
            coordinate = responseCoordinate;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound("User fault: invalid city");
        }

        try
        {
            var dateActual = date == null ? DateTime.Now : date.Value;
            var sunsetDate = await _sunsetProviderProvider.GetSunsetAsync(coordinate, dateActual);
            var solarWatch = _jsonProcessor.Process(sunsetDate);
            solarWatch.City = city;
            solarWatch.Year = DateOnly.FromDateTime(dateActual);
            solarWatch.City = city;


            return Ok(solarWatch);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting weather data");
            return NotFound("Error getting weather data");
        }
    }

    #endregion


    //[HttpGet("GetAll")]
    [HttpGet("GetAll"), Authorize(Roles = "User, Admin")]
    public async Task<IEnumerable<Model.SolarWatch>> GetAll()
    {
       /* var token = Request.Cookies["token"];
        
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Token not found");
            throw new UnauthorizedAccessException("No token found in cookies.");
        }

        var principal = _tokenValidator.GetPrincipalFromToken(token);
        
        if (principal == null)
        {
            throw new UnauthorizedAccessException("Invalid token.");
        }
        
        var roles = principal.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = principal.Identity.Name;
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        Console.WriteLine(email);
*/
      
        var cities = await _cityRepository.GetAllAsync();
        var sunsetSunrises = await _sunriseSunsetRepository.GetAllAsync();

        return sunsetSunrises.Select(s => new Model.SolarWatch
        {
            Year = new DateOnly(s.Sunset.Year, s.Sunset.Month, s.Sunset.Day),
            City = cities.FirstOrDefault(c => c.CityId == s.CityId).Name,
            Sunrise = TimeOnly.FromDateTime(s.Sunrise),
            Sunset = TimeOnly.FromDateTime(s.Sunset),
            CityId = cities.FirstOrDefault(c => c.CityId == s.CityId).CityId,
            SunriseSunsetId = s.SunriseSunsetId
        }).ToList();
    }
    
    [HttpGet("GetByCityDateAsyncDatabase"), Authorize(Roles = "User, Admin")] //explicit kezeli a query paramétereke
    public async Task<ActionResult<Model.SolarWatch>> GetAsyncDatabase(DateTime? date, [Required] string city)
    {
        Console.WriteLine(city);
        Coordinate coordinate = null;
        
        City cityInDb = await _cityRepository.GetByNameAsync(city);
        if (cityInDb == null) // not in db
        {
            try
            {
                var coordinateString = await _coordinateProvider.GetAsyncCoordinate(city);
                var responseCoordinate = _jsonProcessorCoordinate.Process(coordinateString);
                var tempCity = _jsonProcessorCity.Process(coordinateString); // city object mentésre vár
                //Check in db cuz special characters
                bool isInDatabase = await _cityRepository.GetByNameAsync(tempCity.Name) != null; //true 
                Console.WriteLine(isInDatabase);
                if (!isInDatabase) //nics adatbázisban
                {
                    await _cityRepository.AddAsync(tempCity);
                    coordinate = responseCoordinate;
                    cityInDb = await _cityRepository.GetByNameAsync(tempCity.Name);
                }
                else
                {
                    Console.WriteLine("else");
                    cityInDb = await _cityRepository.GetByNameAsync(tempCity.Name);
                    coordinate = new Coordinate(cityInDb.Latitude, cityInDb.Longitude, cityInDb.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound("User fault: invalid city");
            }
        }
        else
        {
            coordinate = new Coordinate(cityInDb.Latitude, cityInDb.Longitude, cityInDb.Name);
        }

        var dateActual = date ?? DateTime.Now;

        var sunriseSunsetInDb =
            await _sunriseSunsetRepository.GetSunriseSunsetByCityAndDateAsync(cityInDb.CityId, dateActual);
        if (sunriseSunsetInDb != null)
        {
            var solarWatchFromDb = new Model.SolarWatch
            {
                City = cityInDb.Name,
                Year = DateOnly.FromDateTime(dateActual),
                Sunrise = TimeOnly.FromDateTime(sunriseSunsetInDb.Sunrise),
                Sunset = TimeOnly.FromDateTime(sunriseSunsetInDb.Sunset),
                CityId = cityInDb.CityId,
                SunriseSunsetId = sunriseSunsetInDb.SunriseSunsetId
                
            };
            return Ok(solarWatchFromDb);
        }

        try
        {
            var sunsetDate = await _sunsetProviderProvider.GetSunsetAsync(coordinate, dateActual);
            var solarWatch = _jsonProcessor.Process(sunsetDate);
            solarWatch.City = cityInDb.Name;
            solarWatch.Year = DateOnly.FromDateTime(dateActual);
            solarWatch.CityId = cityInDb.CityId;


            var newSunriseSunset = new SunriseSunset
            {
                Sunrise = ConvertDate(solarWatch.Sunrise, solarWatch.Year),
                Sunset = ConvertDate(solarWatch.Sunset, solarWatch.Year),
                CityId = cityInDb.CityId
            };

            var sunsetId = await _sunriseSunsetRepository.AddAsync(newSunriseSunset);
            solarWatch.SunriseSunsetId = sunsetId;


            return Ok(solarWatch);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting weather data");
            return NotFound("Error getting weather data");
        }
    }


    [HttpPost("Create"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<Model.SolarWatch>> PostSolarWatchByCityName([Required] DateTime sunrise,
        [Required] DateTime sunset, [Required] string city)
    {
        Coordinate coordinate = null;
        City cityInDb = await _cityRepository.GetByNameAsync(city);
        if (cityInDb == null) // not in db
        {
            try
            {
                //Check the db for saved city-s
                var coordinateString = await _coordinateProvider.GetAsyncCoordinate(city);
                var responseCoordinate = _jsonProcessorCoordinate.Process(coordinateString);
                var tempCity = _jsonProcessorCity.Process(coordinateString); // city object mentésre vár


                bool isInDatabase = await _cityRepository.GetByNameAsync(tempCity.Name) != null; //true 
                Console.WriteLine(isInDatabase);
                if (!isInDatabase) //nics adatbázisban
                {
                    await _cityRepository.AddAsync(tempCity);
                    coordinate = responseCoordinate;
                    cityInDb = await _cityRepository.GetByNameAsync(tempCity.Name);
                }
                else
                {
                    Console.WriteLine("else");
                    cityInDb = await _cityRepository.GetByNameAsync(tempCity.Name);
                    coordinate = new Coordinate(cityInDb.Latitude, cityInDb.Longitude, cityInDb.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound("User fault: invalid city");
            }
        }
     

        // check for saved sunset sunrise in db
        var sunriseSunsetInDb =
            await _sunriseSunsetRepository.GetSunriseSunsetByCityAndDayAsync(cityInDb.CityId, sunset);
        var dateActual = sunrise;
        if (sunriseSunsetInDb != null)
        {
            return BadRequest("there is already set up data for this date");
        }

        var newSunriseSunset = new SunriseSunset
        {
            Sunrise = sunrise,
            Sunset = sunset,
            CityId = cityInDb.CityId
        };
        
        await _sunriseSunsetRepository.AddAsync(newSunriseSunset);
       
        return new Model.SolarWatch
        {
            Year = new DateOnly(dateActual.Year, dateActual.Month, dateActual.Day),
            City = cityInDb.Name,
            Sunrise = new TimeOnly(sunrise.Hour, sunrise.Minute, sunrise.Second),
            Sunset = new TimeOnly(sunset.Hour, sunset.Minute, sunset.Second)
        };
    }
    
    [HttpPost("UpdateCity"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateCity(int id, [FromBody] CityDto cityDto)
    {
        if (cityDto == null)
        {
            return BadRequest();
        }

        var validCity = await _cityRepository.GetByIdAsync(id);
        if (validCity == null)
        {
            return NotFound();
        }
        try
        {
            await _cityRepository.UpdateAsync(id, cityDto);
            var updatedCity = await _cityRepository.GetByIdAsync(id);
            
            return Ok(updatedCity);
        }
        catch (Exception ex)
        {
            
            // _logger.LogError(ex, "Error occurred while updating city.");
            return StatusCode(500, "something went wrong while updating.");
        }
    }

    
    
    
    [HttpPost("UpdateSunsetSunrise"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateSunsetrSunrise(int id, [FromBody] SunriseSunsetDto sunriseSunsetDto)
    {
      
        if (sunriseSunsetDto == null)
        {
            return BadRequest("Invalid data."); // Új ellenőrzés, hogy a bemeneti adat nem null.
        }
        var validData = await _sunriseSunsetRepository.GetByIdAsync(id);
        if (validData == null)
        {
            return NotFound();
        }

        try
        {
            await _sunriseSunsetRepository.UpdateAsync(id, sunriseSunsetDto);
            var updatedSunsetSunrise = await _sunriseSunsetRepository.GetByIdAsync(id);
            return Ok(updatedSunsetSunrise);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "nem mentettem adatbázisba");
        }
    }

    [HttpDelete("DeleteCity"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCity(int id)
    {
        var validData = await _cityRepository.GetByIdAsync(id);
        if (validData == null)
        {
            return BadRequest("Invalid id");
        }

        try
        {
            await _cityRepository.DeleteAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, "did not delete");
        }

    }
    [HttpDelete("DeleteSunriseSunset"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteSunsetSunrise(int id)
    {
        var validData = await _sunriseSunsetRepository.GetByIdAsync(id);
        if (validData == null)
        {
            return BadRequest("Invalid id");
        }

        try
        {
            await _sunriseSunsetRepository.DeleteAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, "did not delete");
        }

    }

   
    [HttpGet("GetCity"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<City?>> GetCityByName([Required]string cityStr)
    {
        var city = await _cityRepository.GetByNameAsync(cityStr);
        if (city==null)
        {
            return BadRequest("not found");
        }
        

        return Ok(city);
    }

    [HttpGet("GetSunriseSunset"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<SunriseSunset>> GetSunriseSunset(string cityName, [Required] TimeOnly sunrise, [Required] DateTime date)
    {
        try
        {
            DateTime dateTime = date.Date + sunrise.ToTimeSpan();
            var sunsetSunrise = await _sunriseSunsetRepository.GetByNameAsync(cityName, dateTime);
            if (sunsetSunrise == null)
            {
                return BadRequest("not found");
            }
            Console.WriteLine(sunsetSunrise.Sunrise);

            var result = new
            {
                SunriseSunsetId = sunsetSunrise.SunriseSunsetId,
                Sunrise = sunsetSunrise.Sunrise,
                Sunset = sunsetSunrise.Sunset,
                CityId = sunsetSunrise.CityId
            };

            return Ok(result);
        }
        catch(Exception e)
        {
            return StatusCode(500, "something went wrong");
        }
    }

    


    private DateTime ConvertDate(TimeOnly solarWatchSunrise, DateOnly solarWatchYear)
    {
        return solarWatchYear.ToDateTime(solarWatchSunrise);
    }
}