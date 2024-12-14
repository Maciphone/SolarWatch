using Microsoft.EntityFrameworkCore;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Service.Repository;

public class CityRepository : ICityRepository<City, CityDto>
{
    private ApplicationDbContext _context;
    private readonly ILogger<CityRepository> _logger; //registrate!!!
    private readonly IJsonProcessor<City> _jsonProcessor; //registrate!!!

    public CityRepository(ApplicationDbContext context, ILogger<CityRepository> logger, IJsonProcessor<City> jsonProcessor)
    {
        _context = context;
        _logger = logger;
        _jsonProcessor = jsonProcessor;
    }
    
    public async Task<IEnumerable<City>> GetAllAsync()
    {
        return await _context.Cities.ToListAsync();
    }

    public async Task<City?> GetByIdAsync(int id)
    {
        return await _context.Cities.FirstOrDefaultAsync(c => c.CityId == id);
    }

    public async Task<City?> GetByNameAsync(string city,DateTime? dateTime)
    {
        try
        {
            return await _context.Cities.FirstOrDefaultAsync(c => c.Name == city);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting city by name: {city}");
            return null;
        }
    }

    public async Task<int> AddAsync(City t)
    {
        try
        {
            await _context.Cities.AddAsync(t);
            await _context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding city: {t.Name}");
        }
        return t.CityId;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Cities.FindAsync(id);
        if (entity != null)
        {
            _context.Cities.Remove(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidCastException("not found");
        }
    }

    public async Task UpdateAsync(int id, CityDto t)
    {
        try
        {
            var cityInDb = await _context.Cities.FindAsync(id);
            if (cityInDb == null)
            {
                throw new InvalidOperationException("city not found");
            }

            cityInDb.Name = t.Name;
            cityInDb.Latitude = t.Latitude;
            cityInDb.Longitude = t.Longitude;
            cityInDb.State = t.State;
            cityInDb.Country = t.Country;
            
            _context.Cities.Update(cityInDb);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating city: {t.Name}");
        }
    }

  
    public async Task<City?> GetSunriseSunsetByCityAndDateAsync(int cityId, DateTime date)
    {
        // Implementáció szükséges
        return await Task.FromResult<City?>(null);
    }

    public Task<City?> GetSunriseSunsetByCityAndDayAsync(int cityId, DateTime date)
    {
        throw new NotImplementedException();
    }


    /*
    public IEnumerable<City> GetAll()
    {
        var all = _context.Cities.ToList();
        return all;
    }

    public City? GetById(int id)
    {
        var search = _context.Cities.FirstOrDefault(c => c.CityId == id);
        return search;
    }

    public async Task<City?> getByName(string city)
    {
        var result = await _context.Cities.FirstOrDefaultAsync(c => c.Name == city);
        
        return result;
    }

    public  void Add(City t)
    {
        _context.Cities.Add(t);
      _context.SaveChanges();
    }

    public void Delete(City t)
    {
        _context.Cities.Remove(t);
        _context.SaveChanges();
        
    }

    public  void Update(City t)
    {
        _context.Cities.Update(t);
         _context.SaveChanges();
        
    }

    public Task<City?> GetSunriseSunsetByCityAndDateAsync(int cityId, DateTime date)
    {
        return null;
    }
    */
}