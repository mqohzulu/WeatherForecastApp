# Weather Forecast API

Weather Forecast API is an ASP.NET Core Web API that fetches weather data from OpenWeatherMap and stores forecast records in an in-memory SQLite database.

## Features

- Fetch real-time weather from OpenWeatherMap
- Store weather data in an in-memory SQLite database
- REST API with Swagger documentation
- Uses `HttpClientFactory` for external API calls
- Uses dependency injection throughout the application

## Tech Stack

- ASP.NET Core
- Entity Framework Core
- SQLite (in-memory)
- HttpClientFactory
- Swagger (Swashbuckle)

## Installation and Setup

1. Clone the repository:

```bash
git clone https://github.com/your-username/weather-forecast-api.git
cd weather-forecast-api
```

2. Configure API keys in `appsettings.json`:

```json
{
  "WeatherApi": {
    "ApiKey": "your_openweathermap_api_key",
    "BaseUrl": "https://api.openweathermap.org/data/2.5/weather"
  }
}
```

3. Run the application:

```bash
dotnet run
```

## API Endpoints

- `GET /WeatherForecast?location={city}`: Fetch weather data for a location
- `POST /WeatherForecast`: Save weather forecast to the database
- `GET /swagger`: Open Swagger UI

## Environment Variables

- `WeatherApi:ApiKey`: OpenWeatherMap API key
- `WeatherApi:BaseUrl`: Base URL for the weather API

## Development and Contribution

1. Fork the repository
2. Create a new branch (`feature/my-feature`)
3. Commit your changes
4. Push to your branch
5. Open a pull request

## Contact

- Email: mqohzulu@gmail.com
- GitHub: https://github.com/mqohzulu/
