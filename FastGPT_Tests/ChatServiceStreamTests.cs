using FastGPT.Dto;
using FastGPT.Dto.Chat;
using FastGPT.HttpApi;
using FastGPT.Services;
using Moq;
using System.Net.ServerSentEvents;
using System.Text;

namespace FastGPT_Tests
{
    public class ChatServiceStreamTests
    {
        private readonly Mock<IChatApi> _mockChatApi;
        private readonly ChatService _chatService;

        public ChatServiceStreamTests()
        {
            _mockChatApi = new Mock<IChatApi>();
            _chatService = new ChatService(_mockChatApi.Object);
        }

        [Fact]
        public async Task ChatStreamAsync_WithMessage_ShouldReturnSseItems()
        {
            var sseData = "data: {\"text\":\"Hello\"}\nevent: answer\n\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(sseData));
            
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatStreamRequest>(), default))
                       .ReturnsAsync(stream);

            var results = new List<SseItem<object?>>();
            await foreach (var item in _chatService.ChatStreamAsync("testApp", "test message"))
            {
                results.Add(item);
            }

            Assert.NotEmpty(results);
        }

        [Fact]
        public async Task ChatStreamWithImageAsync_ShouldCallWithImageContent()
        {
            var sseData = "data: [DONE]\nevent: answer\n\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(sseData));
            
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatStreamRequest>(), default))
                       .ReturnsAsync(stream);

            var results = new List<SseItem<object?>>();
            await foreach (var item in _chatService.ChatStreamWithImageAsync("testApp", "http://example.com/image.jpg"))
            {
                results.Add(item);
            }

            _mockChatApi.Verify(x => x.ChatAsync("testApp", It.Is<ChatStreamRequest>(r => 
                r.Messages != null && r.Messages.Count == 1 && r.Messages[0] is ChatContentMessage), default), Times.Once);
        }

        [Fact]
        public async Task ChatStreamWithFileAsync_ShouldCallWithFileContent()
        {
            var sseData = "data: [DONE]\nevent: answer\n\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(sseData));
            
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatStreamRequest>(), default))
                       .ReturnsAsync(stream);

            var results = new List<SseItem<object?>>();
            await foreach (var item in _chatService.ChatStreamWithFileAsync("testApp", "test.pdf", "http://example.com/file.pdf"))
            {
                results.Add(item);
            }

            _mockChatApi.Verify(x => x.ChatAsync("testApp", It.Is<ChatStreamRequest>(r => 
                r.Messages != null && r.Messages.Count == 1 && r.Messages[0] is ChatContentMessage), default), Times.Once);
        }

        [Fact]
        public async Task RequestPluginStreamAsync_ShouldCallWithVariables()
        {
            var variables = new Dictionary<string, object> { { "key", "value" } };
            var sseData = "data: [DONE]\nevent: answer\n\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(sseData));
            
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatStreamRequest>(), default))
                       .ReturnsAsync(stream);

            var results = new List<SseItem<object?>>();
            await foreach (var item in _chatService.RequestPluginStreamAsync("testApp", variables, default))
            {
                results.Add(item);
            }

            _mockChatApi.Verify(x => x.ChatAsync("testApp", It.Is<ChatStreamRequest>(r => 
                r.Variables == variables && r.Messages == null), default), Times.Once);
        }

        [Fact]
        public async Task ChatStreamInteractiveAsync_UserSelect_ShouldSucceed()
        {
            var interactive = new Interactive
            {
                Type = "userSelect",
                Params = new InteractiveParams
                {
                    UserSelectOptions = [new UserSelectOption { Value = "option1" }]
                }
            };
            var sseData = "data: [DONE]\nevent: answer\n\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(sseData));
            
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatStreamRequest>(), default))
                       .ReturnsAsync(stream);

            var results = new List<SseItem<object?>>();
            await foreach (var item in _chatService.ChatStreamInteractiveAsync("testApp", interactive, "chat1", "option1", []))
            {
                results.Add(item);
            }

            Assert.NotEmpty(results);
        }

        [Fact]
        public async Task ChatStreamInteractiveAsync_UserInput_ShouldSucceed()
        {
            var interactive = new Interactive
            {
                Type = "userInput",
                Params = new InteractiveParams
                {
                    InputForm = [new InputFormItem { Key = "field1", Required = true }]
                }
            };
            var form = new Dictionary<string, object> { { "field1", "value1" } };
            var sseData = "data: [DONE]\nevent: answer\n\n";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(sseData));
            
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatStreamRequest>(), default))
                       .ReturnsAsync(stream);

            var results = new List<SseItem<object?>>();
            await foreach (var item in _chatService.ChatStreamInteractiveAsync("testApp", interactive, "chat1", null, form))
            {
                results.Add(item);
            }

            Assert.NotEmpty(results);
        }
    }
}