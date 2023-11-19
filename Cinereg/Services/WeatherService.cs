using Cinereg.Data;
using Cinereg.Entities;

namespace Cinereg.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient client = new();

        public WeatherService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<WeatherForecastDTO> GetWeatherForecast(string location)
        {
            var weatherApiKey = _config["Weather:ServiceApiKey"];
            WeatherForecastDTO weatherForecast = new();
            string requestUri = $"https://api.openweathermap.org/data/2.5/forecast?q={location}&units=metric&appid={weatherApiKey}";

            HttpResponseMessage response = await client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode) weatherForecast = await response.Content.ReadFromJsonAsync<WeatherForecastDTO>();
            return weatherForecast;
        }
    }
}
