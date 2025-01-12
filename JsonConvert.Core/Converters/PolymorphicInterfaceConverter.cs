using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonConvert.Core.Converters;

/// <summary>
/// If an interface can have different implementations, add logic to recognize a particular type of implementation.
/// Converter registration in options: options.Converters.Add(new PolymorphicInterfaceConverter TInterface ());
/// JSON must contain a type indicator, such as "Type": "Namespace.ClassName, AssemblyName".
/// </summary>
/// <typeparam name="TInterface"></typeparam>
public class PolymorphicInterfaceConverter<TInterface> : JsonConverter<TInterface>
{
    public override TInterface Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDoc.RootElement;

        if (!rootElement.TryGetProperty("Type", out var typeProperty))
        {
            throw new JsonException("Missing type discriminator.");
        }

        var typeName = typeProperty.GetString()!;
        var type = Type.GetType(typeName);
            
        if (type == null || !typeof(TInterface).IsAssignableFrom(type))
        {
            throw new JsonException($"Unknown type: {typeName}");
        }

        return (TInterface)JsonSerializer.Deserialize(rootElement.GetRawText(), type, options)!;

    }

    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
    {
        if (value != null)
        {
            var type = value.GetType();
            var json = JsonSerializer.Serialize(value, type, options);
            using var jsonDoc = JsonDocument.Parse(json);

            writer.WriteStartObject();

            writer.WriteString("Type", type.AssemblyQualifiedName);
            foreach (var property in jsonDoc.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }
        }

        writer.WriteEndObject();
    }
}