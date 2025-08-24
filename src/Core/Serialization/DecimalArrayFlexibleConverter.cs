using System.Text.Json;
using System.Text.Json.Serialization;

namespace PeachClient.Serialization;

public class DecimalArrayFlexibleConverter : JsonConverter<decimal[]>
{
    public override decimal[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray) {
            var list = new List<decimal>();
            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                list.Add(reader.GetDecimal());
            }
            return list.ToArray();
        }
        else if (reader.TokenType == JsonTokenType.Number) {
            return new[] { reader.GetDecimal() };
        }
        else if (reader.TokenType == JsonTokenType.String && decimal.TryParse(reader.GetString(), out var d)) {
            return new[] { d };
        }

        throw new JsonException($"Unexpected token parsing decimal[]: {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, decimal[] value, JsonSerializerOptions options)
    {
        if (value.Length == 1) {
            writer.WriteNumberValue(value[0]);
        }
        else {
            writer.WriteStartArray();
            foreach (var v in value)
                writer.WriteNumberValue(v);
            writer.WriteEndArray();
        }
    }
}
