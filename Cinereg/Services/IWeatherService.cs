using Cinereg.Entities;

namespace Cinereg.Services
{
    public interface IWeatherService
    {
        Task<WeatherForecastDTO> GetWeatherForecast(string location);
    }
}
