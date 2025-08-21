using System.Text.Encodings.Web;
using System.Text.Json;

namespace FastGPT.Options
{
    public static class FastGptJsonOptions
    {
        public readonly static JsonSerializerOptions Options = new()
        {
            TypeInfoResolver = FastGPTJsonContext.Default,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
