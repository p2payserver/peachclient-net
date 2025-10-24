//using System.Text.Json;
//using System.Text.Json.Serialization;
//using PeachClient.Models;

//namespace PeachClient.Serialization;

//public class OfferTypeFilterConverter : JsonConverter<OfferTypeFilter>
//{
//    public override OfferTypeFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        var value = reader.GetString();
//        return value?.ToLowerInvariant() switch
//        {
//            "" or null => OfferTypeFilter.All,
//            "ask" => OfferTypeFilter.Ask,
//            "bid" => OfferTypeFilter.Bid,
//            _ => throw new JsonException($"Invalid OfferTypeFilter value: {value}.")
//        };
//    }

//    public override void Write(Utf8JsonWriter writer, OfferTypeFilter value, JsonSerializerOptions options)
//    {
//        writer.WriteStringValue(value switch
//        {
//            OfferTypeFilter.All => "",
//            OfferTypeFilter.Ask => "ask",
//            OfferTypeFilter.Bid => "bid",
//            _ => throw new JsonException($"Invalid OfferTypeFilter value: {value}.")
//        });
//    }
//}
