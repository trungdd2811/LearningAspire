using System.Text.Json.Serialization;
namespace LearningAspire.Commons
{

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(object))]
    public partial class CustomJsonSerializerContext : JsonSerializerContext
    {
    }

}
