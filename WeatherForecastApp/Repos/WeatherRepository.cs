using Microsoft.EntityFrameworkCore;
using WeatherForecastApp.Models;
using WeatherForecastApp.Repos.Interfaces;

namespace WeatherForecastApp.Repos
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using WeatherForecastApp.Models;
    using WeatherForecastApp.Repos.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherDbContext _context;
        private readonly ILogger<WeatherRepository> _logger;

        public WeatherRepository(WeatherDbContext context, ILogger<WeatherRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveWeather(WeatherForecast weather)
        {
            try
            {
                _logger.LogInformation("Saving weather data for location: {Location}, Date: {Date}", weather.Location, weather.Date);
                await _context.WeatherForecasts.AddAsync(weather);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Weather data saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving weather data.");
                throw;
            }
        }

        public async Task<WeatherForecast> GetWeatherFromDbByID(int Id)
        {
            try
            {
                _logger.LogInformation("Fetching weather data for ID: {Id}", Id);
                var weather = await _context.WeatherForecasts.FirstOrDefaultAsync(x => x.Id == Id);

                if (weather == null)
                {
                    _logger.LogWarning("No weather data found for ID: {Id}", Id);
                }
                else
                {
                    _logger.LogInformation("Weather data retrieved successfully for ID: {Id}", Id);
                }

                return weather;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching weather data for ID: {Id}", Id);
                throw;
            }
        }
    }

}
