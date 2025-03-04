using Microsoft.EntityFrameworkCore;
using WeatherForecastApp.Models;

namespace WeatherForecastApp
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
        {

        }
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WeatherForecast>()
                .HasKey(w => w.Id);
        }

    }
}
