using System.Text.Json;
using JsonConvert.Core.Converters;
using JsonConvert.Core.Tests.Helpers;

namespace JsonConvert.Core.Tests;

[TestFixture]
public class PolymorphicInterfaceConverterTests
{
    private JsonSerializerOptions _serializerOptions;
    
    [SetUp]
    public void SetUp()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            Converters = { new PolymorphicInterfaceConverter<IAnimal>() },
            PropertyNameCaseInsensitive = true
        };
    }

    [Test]
    public void Serialize_DogObject_AddsTypeDiscriminator()
    {
        // Tests if serializing a Dog object adds a Type key indicating the type.
        
        // Arrange
        Dog dog = new Dog { Name = "Buddy", IsBarking = true };

        // Act
        string json = JsonSerializer.Serialize<IAnimal>(dog, _serializerOptions);

        // Assert
        Assert.That(json, Does.Contain("\"Type\""));
        Assert.That(json, Does.Contain("\"Buddy\""));
        Assert.That(json, Does.Contain("\"IsBarking\":true"));
    }

    [Test]
    public void Serialize_CatObject_AddsTypeDiscriminator()
    {
        // Tests serializing a Cat object and checks for the presence of the Type key.
        
        // Arrange
        Cat cat = new Cat { Name = "Whiskers", Lives = 9 };

        // Act
        string json = JsonSerializer.Serialize<IAnimal>(cat, _serializerOptions);

        // Assert
        Assert.That(json, Does.Contain("\"Type\""));
        Assert.That(json, Does.Contain("\"Whiskers\""));
        Assert.That(json, Does.Contain("\"Lives\":9"));
    }

    [Test]
    public void Deserialize_DogJson_CreatesDogObject()
    {
        // Verifies that JSON containing the Dog type deserializes correctly into a Dog object.
        
        // Arrange
        string json = "{\"Type\":\"JsonConvert.Core.Tests.Helpers.Dog, JsonConvert.Core.Tests\",\"Name\":\"Buddy\",\"IsBarking\":true}";

        // Act
        IAnimal? result = JsonSerializer.Deserialize<IAnimal>(json, _serializerOptions);

        // Assert
        Assert.That(result, Is.InstanceOf<Dog>());
        Dog dog = (Dog)result!;
        Assert.That(dog.Name, Is.EqualTo("Buddy"));
        Assert.That(dog.IsBarking, Is.True);
    }

    [Test]
    public void Deserialize_CatJson_CreatesCatObject()
    {
        // Verifies that JSON containing the Cat type deserializes correctly into a Cat object.
        
        // Arrange
        string json = "{\"Type\":\"JsonConvert.Core.Tests.Helpers.Cat, JsonConvert.Core.Tests\",\"Name\":\"Whiskers\",\"Lives\":9}";

        // Act
        IAnimal? result = JsonSerializer.Deserialize<IAnimal>(json, _serializerOptions);

        // Assert
        Assert.That(result, Is.InstanceOf<Cat>());
        Cat cat = (Cat)result!;
        Assert.That(cat.Name, Is.EqualTo("Whiskers"));
        Assert.That(cat.Lives, Is.EqualTo(9));
    }

    [Test]
    public void Deserialize_UnknownType_ThrowsJsonException()
    {
        // Tests if deserializing JSON with an unknown type throws a JsonException.
        
        // Arrange
        string json = "{\"Type\":\"UnknownNamespace.UnknownType\",\"Name\":\"Ghost\"}";

        // Act & Assert
        JsonException? exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<IAnimal>(json, _serializerOptions)
        );

        Assert.That(exception.Message, Does.Contain("Unknown type"));
    }

    [Test]
    public void Deserialize_MissingType_ThrowsJsonException()
    {
        // Verifies that missing the Type key in JSON throws a JsonException.
        
        // Arrange
        string json = "{\"Name\":\"Ghost\"}";

        // Act & Assert
        JsonException? exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<IAnimal>(json, _serializerOptions)
        );

        Assert.That(exception.Message, Does.Contain("Missing type discriminator"));
    }
}