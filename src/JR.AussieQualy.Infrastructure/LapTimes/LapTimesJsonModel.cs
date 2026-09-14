namespace JR.AussieQualy.Infrastructure.LapTimes;

using System.Text.Json.Serialization;

public sealed class LapTimesJsonModel
{
    [JsonPropertyName("time")]
    public List<double?> Time { get; init; } = new();

    [JsonPropertyName("lap")]
    public List<int> Lap { get; init; } = new();

    [JsonPropertyName("compound")]
    public List<string> Compound { get; init; } = new();

    [JsonPropertyName("life")]
    public List<int> Life { get; init; } = new();

    [JsonPropertyName("s1")]
    public List<double?> S1 { get; init; } = new();

    [JsonPropertyName("s2")]
    public List<double?> S2 { get; init; } = new();

    [JsonPropertyName("s3")]
    public List<double?> S3 { get; init; } = new();

    [JsonPropertyName("ms1")]
    public List<string> Ms1 { get; init; } = new();

    [JsonPropertyName("ms2")]
    public List<string> Ms2 { get; init; } = new();

    [JsonPropertyName("ms3")]
    public List<string> Ms3 { get; init; } = new();

    [JsonPropertyName("drv")]
    public List<string> DriverCode { get; init; } = new();

    [JsonPropertyName("dNum")]
    public List<int?> DriverNumber { get; init; } = new();

    [JsonPropertyName("team")]
    public List<string> Team { get; init; } = new();

    [JsonPropertyName("wAT")]
    public List<double?> AirTemp { get; init; } = new();

    [JsonPropertyName("wH")]
    public List<double?> Humidity { get; init; } = new();

    [JsonPropertyName("wP")]
    public List<double?> Pressure { get; init; } = new();

    [JsonPropertyName("wR")]
    public List<double?> Rainfall { get; init; } = new();

    [JsonPropertyName("wTT")]
    public List<double?> TrackTemp { get; init; } = new();

    [JsonPropertyName("wWD")]
    public List<double?> WindDir { get; init; } = new();

    [JsonPropertyName("wWS")]
    public List<double?> WindSpeed { get; init; } = new();

    [JsonPropertyName("pos")]
    public List<int?> Pos { get; init; } = new();

    [JsonPropertyName("qs")]
    public List<string> QSegment { get; init; } = new();

    [JsonPropertyName("lST")]
    public List<double> LapStartTime { get; init; } = new();
}
