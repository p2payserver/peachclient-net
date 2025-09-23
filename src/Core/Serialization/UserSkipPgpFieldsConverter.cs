using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using PeachClient.Models;

public class UserSkipPgpFieldsConverter : JsonConverter<User>
{
    public override User? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var user = new User();
        
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();
                
                switch (propertyName)
                {
                    case "id":
                        user.Id = reader.GetString();
                        break;
                    case "creationDate":
                        user.CreationDate = JsonSerializer.Deserialize<DateTime>(ref reader, options);
                        break;
                    case "trades":
                        user.Trades = reader.GetInt32();
                        break;
                    case "rating":
                        user.Rating = reader.GetDouble();
                        break;
                    case "ratingCount":
                        user.RatingCount = reader.GetInt32();
                        break;
                    case "medals":
                        user.Medals = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                        break;
                    case "disputes":
                        user.Disputes = JsonSerializer.Deserialize<Disputes>(ref reader, options);
                        break;
                    case "pgpPublicKey":
                    case "pgpPublicKeyProof":
                    case "pgpPublicKeys":
                        reader.Skip(); // Skip heavy PGP fields
                        break;
                    default:
                        reader.Skip(); // Skip unknown fields
                        break;
                }
            }
        }
        
        return user;
    }

    public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
    {
        throw new UnreachableException();
    }
}
