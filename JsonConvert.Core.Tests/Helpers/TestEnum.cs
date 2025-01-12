using System.Text.Json.Serialization;

namespace JsonConvert.Core.Tests;

public enum TestEnum
{
    [JsonPropertyName("first_value")]
    FirstValue,

    [JsonPropertyName("second_value")]
    SecondValue,

    ThirdValue // Lack of JsonPropertyName, so it should be serialized as "ThirdValue"
}