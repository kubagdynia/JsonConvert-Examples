using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonConvert.Core.Converters;

public class JsonStringEnumConverterWithAttributesGeneric<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private static readonly EnumCache<TEnum> Cache = new();

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var enumValue = reader.GetString();
        if (enumValue != null && Cache.NameToEnumMap.TryGetValue(enumValue, out var result))
        {
            return result;
        }

        throw new JsonException($"Unable to convert \"{enumValue}\" to enum \"{typeof(TEnum)}\".");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (Cache.EnumToNameMap.TryGetValue(value, out var name))
        {
            writer.WriteStringValue(name);
            return;
        }

        throw new JsonException($"Unable to convert \"{value}\" to string for enum \"{typeof(TEnum)}\".");
    }

    private class EnumCache<T> where T : struct, Enum
    {
        public readonly IReadOnlyDictionary<string, T> NameToEnumMap;
        public readonly IReadOnlyDictionary<T, string> EnumToNameMap;

        public EnumCache()
        {
            NameToEnumMap = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(field => new
                {
                    Name = field.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? field.Name,
                    Value = (T)field.GetValue(null)!
                })
                .ToDictionary(x => x.Name, x => x.Value);

            EnumToNameMap = NameToEnumMap.ToDictionary(kv => kv.Value, kv => kv.Key);
        }
    }
}