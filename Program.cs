using System.Net;
using System.Net.Http;
using System.Text.Json;

var handler = new HttpClientHandler
{
    AutomaticDecompression = DecompressionMethods.All
};

using HttpClient client = new HttpClient(handler);

client.Timeout = TimeSpan.FromSeconds(30);
client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

string[] cities = File.ReadAllLines("cities.txt");

List<WeatherData> weatherData = new();

foreach (string city in cities)
{
    if (string.IsNullOrWhiteSpace(city))
        continue;

    string url = $"https://wttr.in/{city}?format=j1";

    Console.WriteLine($"Получаем данные для города: {city}");

    try
    {
        using HttpRequestMessage request = new HttpRequestMessage(
            HttpMethod.Get,
            url
        );

        request.Version = HttpVersion.Version11;
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
        request.Headers.ConnectionClose = true;

        using HttpResponseMessage response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead
        );

        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        WeatherResponse? data =
            JsonSerializer.Deserialize<WeatherResponse>(json);

        if (data == null ||
            data.CurrentCondition.Count == 0 ||
            data.NearestArea.Count == 0)
        {
            Console.WriteLine($"Не удалось получить данные для {city}");
            continue;
        }

        double temperature = double.Parse(
            data.CurrentCondition[0].Temperature
        );

        string country = data.NearestArea[0].Country[0].Value;

        weatherData.Add(new WeatherData
        {
            City = city,
            Country = country,
            Temperature = temperature
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка для {city}: {ex.Message}");
    }
}

Console.WriteLine();
Console.WriteLine("Погода по городам:");
Console.WriteLine();

foreach (WeatherData weather in weatherData)
{
    Console.WriteLine(
        $"{weather.City}, {weather.Country} {weather.Temperature:+0;-0;0}°C"
    );
}