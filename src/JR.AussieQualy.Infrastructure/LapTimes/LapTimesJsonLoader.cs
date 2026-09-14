namespace JR.AussieQualy.Infrastructure.LapTimes;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class LapTimesJsonLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
        ,Converters =
        {
            new NullableDoubleJsonConverter(),
            new NullableIntJsonConverter()
        }
    };

    public static LapTimesJsonModel Load(string path)
    {
        var json = File.ReadAllText(path);

        var model = JsonSerializer.Deserialize<LapTimesJsonModel>(json, Options);

        if (model == null)
        {
            throw new InvalidOperationException("Failed to deserialize lap times JSON.");
        }

        return model;
    }
}
