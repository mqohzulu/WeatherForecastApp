﻿
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherForecastApp.Models;
using WeatherForecastApp.Repos.Interfaces;
using WeatherForecastApp.Services.Interfaces;

namespace WeatherForecastApp.Services
{
    //Gets weather information from external open weather api.
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        public WeatherService(IHttpClientFactory httpClientFactory, IWeatherRepository weatherRepository, WeatherApiOptions options)
        {
            _weatherRepository = weatherRepository;
            _httpClientFactory = httpClientFactory;
            _apiKey = options.ApiKey;
        }
        public async Task<IEnumerable<WeatherForecast>> GetWeather(string location)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient("WeatherService");
                string url = $"?q={location}&appid={_apiKey}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return Enumerable.Empty<WeatherForecast>();
                }
                else
                {
                    var weather = await response.Content.ReadAsStringAsync();
                    var weatherForecast = JObject.Parse(weather);

                    var forecast = new WeatherForecast
                    {
                        Location = $"{weatherForecast["sys"]["country"]}, {location}, Lon: {weatherForecast["coord"]["lon"]}, Lat: {weatherForecast["coord"]["lat"]}",
                        Date = DateTime.UtcNow,
                        TemperatureC = (float)Math.Round(weatherForecast["main"]["temp"].Value<double>() - 273.15, 1), // Convert from Kelvin to Celsius
                        Conditions = weatherForecast["weather"][0]["description"].Value<string>()
                    };
                    return new List<WeatherForecast> { forecast };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting weather data", ex);
            }
           
        }
        public async Task SaveWeather(WeatherForecast weather)
        {
            
            await _weatherRepository.SaveWeather(weather);
        }
        public async Task<WeatherForecast> GetWeatherFromDbByID(int Id)
        {
            return await _weatherRepository.GetWeatherFromDbByID(Id);
        }   
    }
}
