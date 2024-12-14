namespace SolarWatch.Service.Repository;

public interface ICityRepository<T,TR>
{
    
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByNameAsync(string name, DateTime? date = null);
    Task<int> AddAsync(T entity);
    Task DeleteAsync(int id);
    Task UpdateAsync(int id,TR entity);
    Task<T?> GetSunriseSunsetByCityAndDateAsync(int cityId, DateTime date);
    Task<T?> GetSunriseSunsetByCityAndDayAsync(int cityId, DateTime date);
 
    /*IEnumerable<T> GetAll();
    T? GetById(int id);
    Task<T>? getByName(string city);
    void Add(T t);
    void Delete(T t);
    void Update(T t);

    public  Task<T>? GetSunriseSunsetByCityAndDateAsync(int cityId, DateTime date);
    */
    
    
}