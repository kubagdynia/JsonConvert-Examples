using System.Text.Json;
using JsonConvert.Core.Converters;
using JsonConvert.Core.Tests.Helpers;

namespace JsonConvert.Core.Tests;

[TestFixture]
public class InterfaceConverterTests
{
    private JsonSerializerOptions _serializerOptions;

    [SetUp]
    public void SetUp()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            Converters = { new InterfaceConverter<ITestInterface, TestImplementation>() }
        };
    }

    [Test]
    public void Serialize_ImplementationOfInterface_Success()
    {
        // Verifies that serialization of an interface implementation works correctly and generates valid JSON.
        
        // Arrange
        TestImplementation obj = new TestImplementation { Name = "Test", Value = 42 };

        // Act
        string json = JsonSerializer.Serialize<ITestInterface>(obj, _serializerOptions);

        // Assert
        Assert.That(json, Is.EqualTo("{\"Name\":\"Test\",\"Value\":42}"));
    }

    [Test]
    public void Deserialize_ValidJsonToInterface_Success()
    {
        // Verifies that deserialization of JSON to an object implementing an interface works correctly.
        
        // Arrange
        string json = "{\"Name\":\"Test\",\"Value\":42}";

        // Act
        ITestInterface? result = JsonSerializer.Deserialize<ITestInterface>(json, _serializerOptions);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Test"));
        Assert.That(result.Value, Is.EqualTo(42));
    }

    [Test]
    public void Serialize_NullValue_ReturnsNullJson()
    {
        // Verifies that serializing a null value correctly generates null in JSON.
        
        // Arrange
        ITestInterface? obj = null;

        // Act
        string json = JsonSerializer.Serialize(obj, _serializerOptions);

        // Assert
        Assert.That(json, Is.EqualTo("null"));
    }

    [Test]
    public void Deserialize_NullJson_ReturnsNull()
    {
        // Verifies that deserialization of null in JSON returns null.
        
        // Arrange
        string json = "null";

        // Act
        ITestInterface? result = JsonSerializer.Deserialize<ITestInterface>(json, _serializerOptions);

        // Assert
        Assert.That(result, Is.Null);
    }
}