using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonConvert.Core.Converters;

public class InterfaceConverter<TInterface, TImplementation> : JsonConverter<TInterface>
    where TImplementation : TInterface, new()
{
    public override TInterface Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var deserialized = JsonSerializer.Deserialize<TImplementation>(ref reader, options);

        if (deserialized == null)
        {
            throw new JsonException("Deserialization returned null.");
        }

        return deserialized;
    }

    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (TImplementation)value!, options);
    }
}