using Microsoft.EntityFrameworkCore;
using WeatherForecastApp.Models;
using WeatherForecastApp.Repos.Interfaces;

namespace WeatherForecastApp.Repos
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherDbContext _context;
        public WeatherRepository(WeatherDbContext context)
        {
            _context = context;
        }
        public async Task SaveWeather(WeatherForecast weather)
        {
            await _context.WeatherForecasts.AddAsync(weather);
            await _context.SaveChangesAsync();
        }
    }
}
