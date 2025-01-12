using System.Text.Json;
using System.Text.Json.Serialization;
using JsonConvert.Core.Converters;

namespace JsonConvert.Core.Tests;

[TestFixture]
public class JsonStringEnumConverterWithAttributesFactoryTests
{
    private JsonSerializerOptions _serializerOptions;

    [SetUp]
    public void SetUp()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverterWithAttributesFactory() }
        };
    }

    [Test]
    public void Factory_CanConvert_EnumType_ReturnsTrue()
    {
        // Tests if the factory recognizes enum types as supported.
        
        // Arrange
        var factory = new JsonStringEnumConverterWithAttributesFactory();

        // Act
        bool result = factory.CanConvert(typeof(TestEnum));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Factory_CanConvert_NonEnumType_ReturnsFalse()
    {
        // Tests if the factory rejects types other than enums.
        
        // Arrange
        var factory = new JsonStringEnumConverterWithAttributesFactory();

        // Act
        bool result = factory.CanConvert(typeof(string));

        // Assert
        Assert.That(result, Is.False);
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
        // Tests if serialization uses field names if JsonPropertyName is not present.
        
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
        // Tests if deserialization throws an exception for invalid values.
        
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
        // Tests if serialization throws an exception for an enum value outside the range.
        
        // Arrange
        TestEnum invalidEnumValue = (TestEnum)999; // Invalid enum value

        // Act & Assert
        JsonException? exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Serialize(invalidEnumValue, _serializerOptions)
        );

        Assert.That(exception.Message, Does.Contain("Unable to convert"));
    }

    [Test]
    public void Factory_CreateConverter_ValidEnumType_Success()
    {
        // Verifies that the factory can create a converter for an enum type.
        
        // Arrange
        var factory = new JsonStringEnumConverterWithAttributesFactory();

        // Act
        JsonConverter converter = factory.CreateConverter(typeof(TestEnum), _serializerOptions);

        // Assert\
        Assert.That(converter, Is.Not.Null);
        Assert.That(converter, Is.InstanceOf<JsonConverter>());
    }
}