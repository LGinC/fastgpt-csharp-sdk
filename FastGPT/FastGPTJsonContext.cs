using FastGPT.Dto;
using System.Text.Json.Serialization;

namespace FastGPT
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(ChatAnswerResponse))]
    [JsonSerializable(typeof(ChatFlowNodeStatusResponse))]
    [JsonSerializable(typeof(ChatFlowResponse[]))]
    [JsonSerializable(typeof(ChatInteractiveResponse[]))]
    [JsonSerializable(typeof(ChatToolCallResponse))]
    [JsonSerializable(typeof(ChatToolParamsResponse))]
    [JsonSerializable(typeof(ChatToolResponse))]
    [JsonSerializable(typeof(ChatErrorResponse))]
    [JsonSerializable(typeof(Dictionary<string, object>))]
    public partial class FastGPTJsonContext : JsonSerializerContext
    {
    }
}
