using Cinereg.Models;

namespace Cinereg.Services
{
    public interface IWeatherService
    {
        Task<WeatherForecastDTO> GetWeatherForecast(string location);
    }
}
