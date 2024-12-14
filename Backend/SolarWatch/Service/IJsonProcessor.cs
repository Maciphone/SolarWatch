namespace SolarWatch.Service;

public interface IJsonProcessor<T>
{
    T Process(string data);
}