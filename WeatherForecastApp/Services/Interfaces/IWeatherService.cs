using WeatherForecastApp.Models;

namespace WeatherForecastApp.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetWeather(string location);
        Task SaveWeather(WeatherForecast weather);
    }

}
