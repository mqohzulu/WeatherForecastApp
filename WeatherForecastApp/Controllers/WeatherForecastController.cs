using Microsoft.AspNetCore.Mvc;
using WeatherForecastApp.Models;
using WeatherForecastApp.Services.Interfaces;

namespace WeatherForecastApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherService _weatherService;
        public WeatherForecastController(IWeatherService weatherService,ILogger<WeatherForecastController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetWeatherForecast")]
        public async Task<IActionResult> GetWeather(string location)
        {
            try
            {
                _logger.LogInformation("Fetching weather forecast for location: {Location}", location);

                var weather = await _weatherService.GetWeather(location);

                if (weather == null || !weather.Any())
                {
                    _logger.LogWarning("No weather data found for location: {Location}", location);
                    return NotFound(new { message = $"Weather data not found for {location}" });
                }

                _logger.LogInformation("Weather forecast retrieved successfully for {Location}", location);
                return Ok(weather);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching weather data for location: {Location}", location);
                return StatusCode(500, new { message = "An error occurred while retrieving weather data. Please try again later." });
            }
        }

        [HttpPost]
        [Route("SaveWeather")]
        public async Task<IActionResult> SaveWeather([FromBody] WeatherForecast weather)
        {
            try
            {
                if (weather == null)
                {
                    _logger.LogWarning("Invalid weather data received for saving.");
                    return BadRequest(new { message = "Invalid weather data." });
                }

                await _weatherService.SaveWeather(weather);

                return Ok(new { message = "Weather data saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving weather data for location: {Location}", weather?.Location);
                return StatusCode(500, new { message = "An error occurred while saving weather data. Please try again later." });
            }
        }

        [HttpGet]
        [Route("GetWeatherFromDbByID/{Id}")]
        public async Task<IActionResult> GetWeatherById(int Id)
        {
            try
            {
                var weather = await _weatherService.GetWeatherFromDbByID(Id);

                if (weather == null)
                {
                    _logger.LogWarning("Weather data not found for ID: {Id}", Id);
                    return NotFound(new { message = $"Weather data not found for ID: {Id}" });
                }

                return Ok(weather);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching weather data for ID: {Id}", Id);
                return StatusCode(500, new { message = "An error occurred while retrieving weather data. Please try again later." });
            }
        }

    }
}
