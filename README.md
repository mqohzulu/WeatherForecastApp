🌦️ Weather Forecast API
Welcome to the Weather Forecast API! This application provides real-time weather data using OpenWeatherMap and allows users to store and retrieve weather forecasts from a local SQLite database.

📌 Features
✅ Fetch real-time weather from OpenWeatherMap
✅ Store weather data in an in-memory SQLite database
✅ RESTful API with Swagger support
✅ Uses HttpClientFactory for external API calls
✅ Dependency Injection for better maintainability

🛠️ Tech Stack
Technology	Description
🌐 ASP.NET Core	Web API framework
🔍 Entity Framework Core	Database ORM
🗄️ SQLite (In-Memory)	Lightweight database for testing
☁️ HttpClientFactory	For making external API calls
📄 Swagger (Swashbuckle)	API documentation
📦 Installation & Setup
1️⃣ Clone the Repository

sh
Copy
Edit
git clone https://github.com/your-username/weather-forecast-api.git
cd weather-forecast-api
2️⃣ Configure API Keys
Add the following to your appsettings.json:

json
Copy
Edit
{
  "WeatherApi": {
    "ApiKey": "your_openweathermap_api_key",
    "BaseUrl": "https://api.openweathermap.org/data/2.5/weather"
  }
}
3️⃣ Run the Application
sh
Copy
Edit
dotnet run

🔌 API Endpoints
HTTP Method	Endpoint	Description
GET	/WeatherForecast?location={city}	Fetches weather data for a given location
POST	/WeatherForecast	Saves weather forecast to the database
GET	/swagger	Opens Swagger UI for API testing
🛠️ Environment Variables
Variable	Description
WeatherApi:ApiKey	OpenWeatherMap API Key
WeatherApi:BaseUrl	Base URL for weather API
📂 Project Structure
bash
Copy
Edit
📂 WeatherForecastApp
 ┣ 📂 Controllers            # API Controllers
 ┣ 📂 Models                 # Data models
 ┣ 📂 Repos                  # Repository layer
 ┣ 📂 Services               # Business logic services
 ┣ 📂 Data                   # EF Core DB context
 ┣ 📝 WeatherApiOptions.cs   # Api key
 ┣ 📝 Program.cs             # Application entry point
🛠️ Development & Contribution
Fork the repository
Create a new branch (feature/my-feature)
Commit your changes
Push to your branch
Open a Pull Request

🤝 Contact
📧 Email: mqohzulu@gmail.com
🌍 GitHub: https://github.com/mqohzulu/

🚀 Happy coding! 😊







