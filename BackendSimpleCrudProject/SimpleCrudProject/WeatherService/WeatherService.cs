using System.Text.Json;
using NuGet.Protocol;

namespace SimpleCrudProject.WeatherService;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<double?> GetWeatherByCity(string city)
    {
        try
        {
            var url = $"https://api.weatherbit.io/v2.0/current?city={city}&key={_apiKey}";


            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Weatherbit API error: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Weatherbit: {json}");
            
            var data = JsonSerializer.Deserialize<WeatherResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Console.WriteLine(data?.Data?.FirstOrDefault()?.Temp);

            return data?.Data?.FirstOrDefault()?.Temp;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching weather data: {ex.Message}");
            return null;
        }
    }

    private class WeatherResponse
    {
        public List<WeatherData>? Data { get; set; }
    }

    private class WeatherData
    {
        public double? Temp { get; set; }
    }
}