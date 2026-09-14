namespace JR.AussieQualy.Infrastructure.LapTimes;

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

// Converts lap time values from data\session_laptimes.json, where the source data
// mixes JSON types for the same logical value instead of using a single consistent type.
internal class NullableDoubleJsonConverter : JsonConverter<double?>
{
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            // A normal recorded lap, e.g. "time": [ ..., 81.802, ... ]
            case JsonTokenType.Number:
                return reader.GetDouble();

            // A missing lap is encoded as the string "None" in session_laptimes.json,
            // e.g. "time": [ "None", 81.802, ... ]. Other strings (e.g. "81.802") are
            // parsed defensively in case laps arrive as quoted numbers.
            case JsonTokenType.String:
            {
                var s = reader.GetString();
                if (string.IsNullOrEmpty(s))
                    return null;

                if (string.Equals(s, "None", StringComparison.OrdinalIgnoreCase))
                    return null;

                if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var d))
                    return d;

                return null;
            }

            // Some upstream exports encode presence/absence flags as booleans,
            // e.g. "time": [ true, ... ] meaning a lap exists.
            case JsonTokenType.True:
                return 1.0;

            // e.g. "time": [ false, ... ] meaning no lap was recorded.
            case JsonTokenType.False:
                return 0.0;

            // Explicit JSON null, e.g. "time": [ null, ... ] for a missing lap.
            case JsonTokenType.Null:
                return null;

            default:
                throw new JsonException($"Unexpected token parsing double?: {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
