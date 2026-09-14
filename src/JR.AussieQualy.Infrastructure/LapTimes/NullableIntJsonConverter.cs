namespace JR.AussieQualy.Infrastructure.LapTimes;

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class NullableIntJsonConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                try
                {
                    return reader.GetInt32();
                }
                catch (Exception)
                {
                    // Fallback: read as double and convert if it's a whole number
                    var d = reader.GetDouble();
                    if (Math.Abs(d - Math.Round(d)) < 1e-9)
                        return Convert.ToInt32(d);
                    throw new JsonException($"JSON number is not a whole integer: {d}");
                }

            case JsonTokenType.String:
            {
                var s = reader.GetString();
                if (string.IsNullOrEmpty(s))
                    return null;

                if (string.Equals(s, "None", StringComparison.OrdinalIgnoreCase))
                    return null;

                if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                    return i;

                if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var dVal))
                {
                    if (Math.Abs(dVal - Math.Round(dVal)) < 1e-9)
                        return Convert.ToInt32(dVal);
                }

                throw new JsonException($"Unable to parse integer value from string: '{s}'");
            }

            case JsonTokenType.Null:
                return null;

            default:
                throw new JsonException($"Unexpected token parsing int?: {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
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
