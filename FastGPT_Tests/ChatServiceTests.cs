using FastGPT.Dto;
using FastGPT.Dto.Chat;
using FastGPT.HttpApi;
using FastGPT.Services;
using Moq;
using System.Net.ServerSentEvents;
using System.Text.Json;

namespace FastGPT_Tests
{
    public class ChatServiceTests
    {
        private readonly Mock<IChatApi> _mockChatApi;
        private readonly ChatService _chatService;

        public ChatServiceTests()
        {
            _mockChatApi = new Mock<IChatApi>();
            _chatService = new ChatService(_mockChatApi.Object);
        }

        [Fact]
        public void GenerateChatId_ShouldReturnValidGuid()
        {
            var chatId = ChatService.GenerateChatId();
            Assert.True(Guid.TryParse(chatId, out _));
        }

        [Fact]
        public async Task ChatAsync_WithRequest_ShouldCallChatApi()
        {
            var messages = new List<ChatMessage> { new ChatBaseMessage("test") };
            var request = new ChatNoneStreamRequest(messages);
            var expectedResponse = new ChatResponse();
            _mockChatApi.Setup(x => x.ChatAsync("testApp", request, default))
                       .ReturnsAsync(expectedResponse);

            var result = await _chatService.ChatAsync("testApp", request);

            Assert.Equal(expectedResponse, result);
            _mockChatApi.Verify(x => x.ChatAsync("testApp", request, default), Times.Once);
        }

        [Fact]
        public async Task ChatAsync_WithMessage_ShouldCreateRequestAndCallApi()
        {
            var expectedResponse = new ChatResponse();
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatNoneStreamRequest>(), default))
                       .ReturnsAsync(expectedResponse);

            var result = await _chatService.ChatAsync("testApp", "test message");

            Assert.Equal(expectedResponse, result);
            _mockChatApi.Verify(x => x.ChatAsync("testApp", It.Is<ChatNoneStreamRequest>(r => 
                r.Messages != null && r.Messages.Count == 1 && ((ChatBaseMessage)r.Messages[0]).Content == "test message"), default), Times.Once);
        }

        [Fact]
        public async Task ChatWithImageAsync_ShouldCreateImageRequest()
        {
            var expectedResponse = new ChatResponse();
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatNoneStreamRequest>(), default))
                       .ReturnsAsync(expectedResponse);

            var result = await _chatService.ChatWithImageAsync("testApp", "http://example.com/image.jpg");

            Assert.Equal(expectedResponse, result);
            _mockChatApi.Verify(x => x.ChatAsync("testApp", It.Is<ChatNoneStreamRequest>(r => 
                r.Messages != null && r.Messages.Count == 1 && r.Messages[0] is ChatContentMessage), default), Times.Once);
        }

        [Fact]
        public async Task ChatWithFileAsync_ShouldCreateFileRequest()
        {
            var expectedResponse = new ChatResponse();
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatNoneStreamRequest>(), default))
                       .ReturnsAsync(expectedResponse);

            var result = await _chatService.ChatWithFileAsync("testApp", "test.pdf", "http://example.com/file.pdf");

            Assert.Equal(expectedResponse, result);
            _mockChatApi.Verify(x => x.ChatAsync("testApp", It.Is<ChatNoneStreamRequest>(r => 
                r.Messages != null && r.Messages.Count == 1 && r.Messages[0] is ChatContentMessage), default), Times.Once);
        }

        [Fact]
        public async Task RequestPluginAsync_ShouldCallApiWithVariables()
        {
            var variables = new Dictionary<string, object> { { "key", "value" } };
            var expectedResponse = new ChatResponse();
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatNoneStreamRequest>(), default))
                       .ReturnsAsync(expectedResponse);

            var result = await _chatService.RequestPluginAsync("testApp", variables, default);

            Assert.Equal(expectedResponse, result);
            _mockChatApi.Verify(x => x.ChatAsync("testApp", It.Is<ChatNoneStreamRequest>(r => 
                r.Variables == variables && r.Messages == null), default), Times.Once);
        }

        [Fact]
        public async Task ChatInteractiveAsync_UserSelect_ValidOption_ShouldSucceed()
        {
            var interactive = new Interactive
            {
                Type = "userSelect",
                Params = new InteractiveParams
                {
                    UserSelectOptions = [new UserSelectOption { Value = "option1" }]
                }
            };
            var expectedResponse = new ChatResponse();
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatNoneStreamRequest>(), default))
                       .ReturnsAsync(expectedResponse);

            var result = await _chatService.ChatInteractiveAsync("testApp", interactive, "chat1", "option1", []);

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task ChatInteractiveAsync_UserSelect_InvalidOption_ShouldThrow()
        {
            var interactive = new Interactive
            {
                Type = "userSelect",
                Params = new InteractiveParams
                {
                    UserSelectOptions = [new UserSelectOption { Value = "option1" }]
                }
            };

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                _chatService.ChatInteractiveAsync("testApp", interactive, "chat1", "invalid", []));
        }

        [Fact]
        public async Task ChatInteractiveAsync_UserSelect_NullMessage_ShouldThrow()
        {
            var interactive = new Interactive { Type = "userSelect" };

            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _chatService.ChatInteractiveAsync("testApp", interactive, "chat1", null, []));
        }

        [Fact]
        public async Task ChatInteractiveAsync_UserInput_ValidForm_ShouldSucceed()
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
            var expectedResponse = new ChatResponse();
            _mockChatApi.Setup(x => x.ChatAsync("testApp", It.IsAny<ChatNoneStreamRequest>(), default))
                       .ReturnsAsync(expectedResponse);

            var result = await _chatService.ChatInteractiveAsync("testApp", interactive, "chat1", null, form);

            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task ChatInteractiveAsync_UserInput_EmptyForm_ShouldThrow()
        {
            var interactive = new Interactive
            {
                Type = "userInput",
                Params = new InteractiveParams
                {
                    InputForm = [new InputFormItem { Key = "field1", Required = true }]
                }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => 
                _chatService.ChatInteractiveAsync("testApp", interactive, "chat1", null, []));
        }

        [Fact]
        public async Task ChatInteractiveAsync_UserInput_MissingRequiredField_ShouldThrow()
        {
            var interactive = new Interactive
            {
                Type = "userInput",
                Params = new InteractiveParams
                {
                    InputForm = [new InputFormItem { Key = "field1", Required = true }]
                }
            };
            var form = new Dictionary<string, object> { { "field2", "value2" } };

            await Assert.ThrowsAsync<ArgumentException>(() => 
                _chatService.ChatInteractiveAsync("testApp", interactive, "chat1", null, form));
        }

        [Fact]
        public async Task ChatInteractiveAsync_UnsupportedType_ShouldThrow()
        {
            var interactive = new Interactive { Type = "unsupported" };

            await Assert.ThrowsAsync<NotSupportedException>(() => 
                _chatService.ChatInteractiveAsync("testApp", interactive, "chat1", null, []));
        }
    }
}