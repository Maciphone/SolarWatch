using Microsoft.EntityFrameworkCore;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Service.Repository;

public class SunriseSunsetRepository : ICityRepository<SunriseSunset, SunriseSunsetDto>
{
    private ApplicationDbContext _context;

    public SunriseSunsetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SunriseSunset>> GetAllAsync()
    {
        return await _context.SunriseSunsets.ToListAsync();
    }

    public async Task<SunriseSunset?> GetByIdAsync(int id)
    {
        return await _context.SunriseSunsets.FirstOrDefaultAsync(s => s.SunriseSunsetId == id);
    }

    public async Task<int> AddAsync(SunriseSunset t)
    {
        await _context.SunriseSunsets.AddAsync(t);
        await _context.SaveChangesAsync();
        return t.SunriseSunsetId;
    }

    public async Task DeleteAsync(int id)
    {
     
            var entity = await _context.SunriseSunsets.FindAsync(id);
            if (entity != null)
            {
                _context.SunriseSunsets.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidCastException("not found");
            }
     
    }

    public async Task UpdateAsync(int id, SunriseSunsetDto sunriseSunsetDto)
    {
        
            var sunriseSunset = await _context.SunriseSunsets.FindAsync(id);
            if (sunriseSunset == null)
            {
                throw new InvalidOperationException("city not found");
            }

            sunriseSunset.Sunrise = sunriseSunsetDto.Sunrise;
            sunriseSunset.Sunset = sunriseSunsetDto.Sunset;
           
            
            _context.SunriseSunsets.Update(sunriseSunset);
            await _context.SaveChangesAsync();
        

      
    }

    public async Task<SunriseSunset?> GetByNameAsync(string cityStr,DateTime? sunrise )
    {
        //most lovely solution
        var sunriseTest = await _context.SunriseSunsets
            .Include(s => s.City)
            .FirstOrDefaultAsync(s => s.Sunrise.Date == sunrise.Value.Date && s.City.Name==cityStr);
        if (sunriseTest == null)
        {
            throw new InvalidOperationException("city not found");
        }

        return sunriseTest;
      
        // var citySunrise = await _context.Cities
        //           .Include(c => c.SunriseSunsets)
        //           .FirstOrDefaultAsync(c => c.Name == cityStr);
        //       if (citySunrise==null)
        //       {
        //           throw new InvalidOperationException("City not found");
        //       }
        //       return citySunrise.SunriseSunsets.FirstOrDefault(s => s.Sunrise == sunrise);
        //
        // var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == cityStr);
        // if (city == null)
        // {
        //     throw new InvalidOperationException("city not found");
        // }
        // return await _context.SunriseSunsets.FirstOrDefaultAsync(s => s.CityId == city.CityId &&
        //                                                               s.Sunrise==sunrise);//check date format!!!
    }

    public async Task<SunriseSunset?> GetSunriseSunsetByCityAndDateAsync(int cityId, DateTime date)
    {
        return await _context.SunriseSunsets
            .FirstOrDefaultAsync(s => s.CityId == cityId && s.Sunrise.Date == date.Date);
    }

    public async Task<SunriseSunset?> GetSunriseSunsetByCityAndDayAsync(int cityId, DateTime date)
    {
        return await _context.SunriseSunsets
            .FirstOrDefaultAsync(s => s.CityId == cityId
                                      && s.Sunrise.Date.Day == date.Day
                                      && s.Sunrise.Date.Year == date.Year
                                      && s.Sunrise.Date.Month == date.Month);
    }


    /*
    public IEnumerable<SunriseSunset> GetAll()
    {
        var all = _context.SunriseSunsets.ToList();
        return all;
    }

    public SunriseSunset? GetById(int id)
    {
        var search = _context.SunriseSunsets.FirstOrDefault(s => s.SunriseSunsetId == id);
        return search;
    }

    public void Add(SunriseSunset t)
    {
        _context.SunriseSunsets.Add(t);
        _context.SaveChanges();
    }

    public void Delete(SunriseSunset t)
    {
        _context.SunriseSunsets.Remove(t);
        _context.SaveChanges();
    }

    public void Update(SunriseSunset t)
    {
        _context.SunriseSunsets.Update(t);
        _context.SaveChanges();
    }

    public async Task<SunriseSunset?> getByName(string city)
    {
        throw new NotImplementedException();
    }

    public async Task<SunriseSunset?> GetSunriseSunsetByCityAndDateAsync(int cityId, DateTime date)
    {
       var result =  await _context.SunriseSunsets
            .FirstOrDefaultAsync(s => s.CityId == cityId && s.Sunrise.Date == date.Date);
       return result;
    }

    */
}