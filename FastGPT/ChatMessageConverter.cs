using FastGPT.Dto.Chat;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FastGPT
{
    public class ChatMessageConverter : JsonConverter<ChatMessage>
    {
        public override void Write(Utf8JsonWriter writer, ChatMessage value, JsonSerializerOptions options)
        {
            var namingPolicy = options.PropertyNamingPolicy;

            writer.WriteStartObject();

            writer.WriteString(namingPolicy?.ConvertName(nameof(ChatMessage.Role)) ?? "role", value.Role);

            switch (value)
            {
                case ChatBaseMessage baseMessage:
                    writer.WriteString(namingPolicy?.ConvertName(nameof(ChatBaseMessage.Content)) ?? "content", baseMessage.Content);
                    break;
                case ChatContentMessage contentMessage:
                    writer.WritePropertyName(namingPolicy?.ConvertName(nameof(ChatContentMessage.Content)) ?? "content");
                    JsonSerializer.Serialize(writer, contentMessage.Content, options);
                    break;
                default:
                    // Handle other potential derived types or throw an exception
                    break;
            }

            writer.WriteEndObject();
        }

        public override ChatMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // For now, deserialization is not the focus.
            throw new NotImplementedException();
        }
    }
}
