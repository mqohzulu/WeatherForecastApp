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
               var weather = await _weatherService.GetWeather(location);
                return Ok(weather);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        
        }

        [HttpPost]
        [Route("SaveWeather")]
        public async Task<IActionResult> SaveWeather(WeatherForecast weather)
        {
            try
            {
                await _weatherService.SaveWeather(weather);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("GetWeatherFromDbByID")]
        public async Task<IActionResult> GetWeatherById(int Id)
        {
            try
            {
                var weather = await _weatherService.GetWeatherFromDbByID(Id);
                return Ok(weather);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }   
    }
}
