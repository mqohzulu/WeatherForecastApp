using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WeatherForecastApp.Models
{
    public class WeatherForecast
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public float TemperatureC { get; set; }
        public string Conditions { get; set; }
        public string Location { get; set; }
    }
}
