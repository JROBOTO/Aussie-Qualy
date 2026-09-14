namespace JR.AussieQualy.Infrastructure.LapTimes;

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class NullableDoubleJsonConverter : JsonConverter<double?>
{
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetDouble();

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
