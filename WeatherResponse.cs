using System.Text.Json.Serialization;

public class WeatherResponse
{
    [JsonPropertyName("current_condition")]
    public List<CurrentCondition> CurrentCondition { get; set; } = new();

    [JsonPropertyName("nearest_area")]
    public List<NearestArea> NearestArea { get; set; } = new();
}

public class CurrentCondition
{
    [JsonPropertyName("temp_C")]
    public string Temperature { get; set; } = "";
}

public class NearestArea
{
    [JsonPropertyName("country")]
    public List<Country> Country { get; set; } = new();
}

public class Country
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = "";
}