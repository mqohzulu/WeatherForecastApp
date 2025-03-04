using Microsoft.EntityFrameworkCore;
using WeatherForecastApp;
using WeatherForecastApp.Repos;
using WeatherForecastApp.Repos.Interfaces;
using WeatherForecastApp.Services;
using WeatherForecastApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlite("DataSource=:memory:"));

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
    dbContext.Database.OpenConnection();
    dbContext.Database.EnsureCreated();
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

app.Run();
