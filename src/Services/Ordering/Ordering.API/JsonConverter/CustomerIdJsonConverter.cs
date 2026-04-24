using Ordering.Domain.ValueObjects.IdValueObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ordering.API.JsonConverter
{
    public class CustomerIdJsonConverter : JsonConverter<CustomerId>
    {
        public override CustomerId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var guid = reader.GetGuid();
            return CustomerId.New(guid);
        }
        public override void Write(Utf8JsonWriter writer, CustomerId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
