using Microsoft.EntityFrameworkCore;
using WeatherForecastApp;
using WeatherForecastApp.Models;
using WeatherForecastApp.Repos;
using WeatherForecastApp.Repos.Interfaces;
using WeatherForecastApp.Services;
using WeatherForecastApp.Services.Interfaces;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "DataSource=:memory:";
var keepAliveConnection = new SqliteConnection(connectionString);
keepAliveConnection.Open();

builder.Services.AddDbContext<WeatherDbContext>(options =>
{
    options.UseSqlite(keepAliveConnection);
});


// Add services to the container.
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddTransient<IWeatherService, WeatherService>();
builder.Services.AddTransient<IWeatherRepository, WeatherRepository>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
    dbContext.Database.EnsureCreated();
    if (!dbContext.WeatherForecasts.Any())
    {
        dbContext.WeatherForecasts.Add(new WeatherForecast
        {
            Id = 1,
            Date = DateTime.Now,
            TemperatureC = 25,
            Location = "MZUMBE",
            Conditions = "Warm - MQ Testing"
        });
        dbContext.SaveChanges();
        Console.WriteLine($"Test data saved. Count: {dbContext.WeatherForecasts.Count()}");

    }

}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStopped.Register(() =>
{
    keepAliveConnection?.Close();
    keepAliveConnection?.Dispose();
});

app.Run();
