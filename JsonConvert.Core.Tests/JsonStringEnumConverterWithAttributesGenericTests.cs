using System.Text.Json;
using JsonConvert.Core.Converters;
using JsonConvert.Core.Tests.Helpers;

namespace JsonConvert.Core.Tests;

[TestFixture]
public class JsonStringEnumConverterWithAttributesGenericTests
{
    private JsonSerializerOptions _serializerOptions;
    
    [SetUp]
    public void SetUp()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverterWithAttributesGeneric<TestEnum>() }
        };
    }
    
    [Test]
    public void Serialize_EnumWithJsonPropertyName_Success()
    {
        // Tests if serialization correctly uses JsonPropertyName value.
        
        // Arrange
        TestEnum value = TestEnum.FirstValue;

        // Act
        string json = JsonSerializer.Serialize(value, _serializerOptions);

        // Assert
        Assert.That(json, Is.EqualTo("\"first_value\""));
    }

    [Test]
    public void Serialize_EnumWithoutJsonPropertyName_UsesFieldName()
    {
        // Verifies that for fields without the JsonPropertyName attribute, the field name is used.
        
        // Arrange
        TestEnum value = TestEnum.ThirdValue;

        // Act
        string json = JsonSerializer.Serialize(value, _serializerOptions);

        // Assert
        Assert.That(json, Is.EqualTo("\"ThirdValue\""));
    }

    [Test]
    public void Deserialize_ValidJsonWithJsonPropertyName_Success()
    {
        // Verifies deserialization of a value defined in JsonPropertyName.
        
        // Arrange
        string json = "\"second_value\"";

        // Act
        TestEnum result = JsonSerializer.Deserialize<TestEnum>(json, _serializerOptions);

        // Assert
        Assert.That(result, Is.EqualTo(TestEnum.SecondValue));
    }

    [Test]
    public void Deserialize_ValidJsonWithoutJsonPropertyName_UsesFieldName()
    {
        // Verifies deserialization of field names without JsonPropertyName.
        
        // Arrange
        string json = "\"ThirdValue\"";

        // Act
        TestEnum result = JsonSerializer.Deserialize<TestEnum>(json, _serializerOptions);

        // Assert
        Assert.That(result, Is.EqualTo(TestEnum.ThirdValue));
    }

    [Test]
    public void Deserialize_InvalidJson_ThrowsJsonException()
    {
        // Verifies that deserialization throws a JsonException for invalid values.
        
        // Arrange
        string json = "\"invalid_value\"";

        // Act & Assert
        JsonException? exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<TestEnum>(json, _serializerOptions)
        );

        Assert.That(exception.Message, Does.Contain("Unable to convert"));
    }

    [Test]
    public void Serialize_InvalidEnumValue_ThrowsJsonException()
    {
        // Tests if serialization throws a JsonException for invalid enum values.
        
        // Arrange
        TestEnum invalidEnumValue = (TestEnum)999; // Invalid enum value

        // Act & Assert
        JsonException? exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Serialize(invalidEnumValue, _serializerOptions)
        );

        Assert.That(exception.Message, Does.Contain("Unable to convert"));
    }
}