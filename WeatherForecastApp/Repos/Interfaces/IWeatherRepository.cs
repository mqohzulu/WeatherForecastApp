using WeatherForecastApp.Models;

namespace WeatherForecastApp.Repos.Interfaces
{
    public interface IWeatherRepository
    {
        Task SaveWeather(WeatherForecast weather);
        Task<WeatherForecast> GetWeatherFromDbByID(int Id);
    }
}
