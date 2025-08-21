using FastGPT.Dto.Chat;
using FastGPT.HttpApi;
using FastGPT.Options;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace FastGPT.Services
{
    public class ChatService(IChatApi chatApi)
    {
        /// <summary>
        /// 生成会话id
        /// </summary>
        /// <returns></returns>
        public static string GenerateChatId() => Guid.CreateVersion7().ToString();

        /// <summary>
        /// 非流式对话
        /// <seealso href="https://doc.fastgpt.cn/docs/introduction/development/openapi/chat#%E5%93%8D%E5%BA%94">响应文档</seealso>
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="request">请求</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ChatResponse> ChatAsync(string appName, ChatNoneStreamRequest request, CancellationToken token = default) =>
            chatApi.ChatAsync(appName, request, token);

        /// <summary>
        /// 非流式对话
        /// <seealso href="https://doc.fastgpt.cn/docs/introduction/development/openapi/chat#%E5%93%8D%E5%BA%94">响应文档</seealso>
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="chatId">会话ID</param>
        /// <param name="detail">是否返回中间值</param>
        /// <param name="responseId">自定义响应id</param>
        /// <param name="varibles">变量列表</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ChatResponse> ChatAsync(string appName, string message, string? chatId = null, bool detail = true, Dictionary<string, object>? varibles = null, string? responseId = null, CancellationToken token = default) =>
            ChatAsync(appName, new ChatNoneStreamRequest([new ChatBaseMessage(message)], chatId, detail, varibles, responseId), token);

        /// <summary>
        /// 流式对话
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        public async IAsyncEnumerable<SseItem<object?>> ChatStreamAsync(string appName, ChatStreamRequest request, [EnumeratorCancellation]CancellationToken token)
        {
            var result = await chatApi.ChatAsync(appName, request, token);
            await foreach (SseItem<string> item in SseParser.Create(result).EnumerateAsync(token))
            {
                //Console.WriteLine(item.Data);
                yield return item.EventType switch
                {
                    ChatEventConsts.Answer =>
                        item.Data is "[DONE]" ? new SseItem<object?>(item.Data, item.EventType)//结束消息原样返回
                        : new SseItem<object?>(JsonSerializer.Deserialize<ChatAnswerResponse>(item.Data, FastGptJsonOptions.Options), item.EventType),
                    ChatEventConsts.FlowNodeStatus => new SseItem<object?>(JsonSerializer.Deserialize<ChatFlowNodeStatusResponse>(item.Data, FastGptJsonOptions.Options), item.EventType),
                    ChatEventConsts.FlowResponses => new SseItem<object?>(JsonSerializer.Deserialize<ChatFlowResponse[]>(item.Data, FastGptJsonOptions.Options), item.EventType),
                    ChatEventConsts.Interactive => new SseItem<object?>(JsonSerializer.Deserialize<ChatInteractiveResponse>(item.Data, FastGptJsonOptions.Options), item.EventType),
                    ChatEventConsts.ToolCall => new SseItem<object?>(JsonSerializer.Deserialize<ChatToolCallResponse>(item.Data, FastGptJsonOptions.Options), item.EventType),
                    ChatEventConsts.ToolParams => new SseItem<object?>(JsonSerializer.Deserialize<ChatToolParamsResponse>(item.Data, FastGptJsonOptions.Options), item.EventType),
                    ChatEventConsts.ToolResponse => new SseItem<object?>(JsonSerializer.Deserialize<ChatToolResponse>(item.Data, FastGptJsonOptions.Options), item.EventType),
                    ChatEventConsts.UpdateVariables => new SseItem<object?>(JsonSerializer.Deserialize<Dictionary<string, object>>(item.Data, FastGptJsonOptions.Options), item.EventType),
                    ChatEventConsts.Error => new SseItem<object?>(JsonSerializer.Deserialize<ChatErrorResponse>(item.Data, FastGptJsonOptions.Options), item.EventType),
                    //还有FastAnswer, 但该事件的data就为string，就不单独处理
                    _ => new SseItem<object?>(item.Data, item.EventType),
                };
            }
        }

        /// <summary>
        /// 文本消息流式对话
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="chatId">会话ID</param>
        /// <param name="detail">是否返回中间值</param>
        /// <param name="responseId">自定义响应id</param>
        /// <param name="varibles">变量列表</param>
        /// <returns></returns>
        public IAsyncEnumerable<SseItem<object?>> ChatStreamAsync(string appName, string message, string? chatId = null, bool detail = true, Dictionary<string, object>? varibles = null, string? responseId = null, CancellationToken token = default)
            => ChatStreamAsync(appName, new ChatStreamRequest([new ChatBaseMessage(message)], chatId, detail, varibles, responseId), token);

        /// <summary>
        /// 图片消息流式对话
        /// </summary>
        /// <param name="imageUrl">图片URL</param>
        /// <param name="chatId">会话ID</param>
        /// <param name="detail">是否返回中间值</param>
        /// <returns></returns>
        public IAsyncEnumerable<SseItem<object?>> ChatStreamWithImageAsync(string appName, string imageUrl, string? chatId = null, bool detail = true, CancellationToken token = default)
            => ChatStreamAsync(appName, new ChatStreamRequest([new ChatContentMessage([ContentItem.FromImage(imageUrl)])], chatId, detail), token);

        /// <summary>
        /// 文件消息流式对话
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileUrl">文件URL</param>
        /// <param name="chatId">会话ID</param>
        /// <param name="detail">是否返回中间值</param>
        /// <returns></returns>
        public IAsyncEnumerable<SseItem<object?>> ChatStreamWithFileAsync(string appName, string fileName, string fileUrl, string? chatId = null, bool detail = true, CancellationToken token = default)
            => ChatStreamAsync(appName, new ChatStreamRequest([new ChatContentMessage([ContentItem.FromFile(fileName, fileUrl)])], chatId, detail), token);

        /// <summary>
        /// 交互式流式对话
        /// </summary>
        /// <param name="interactive">交互对象</param>
        /// <param name="chatId">会话ID</param>
        /// <param name="message">用户选择的消息（userSelect类型使用）</param>
        /// <param name="form">表单数据（userInput类型使用）</param>
        /// <param name="detail">是否返回中间值</param>
        /// <returns></returns>
        public IAsyncEnumerable<SseItem<object?>> ChatStreamInteractiveAsync(string appName, Interactive interactive, string chatId, string? message, Dictionary<string, object> form, bool detail = true, CancellationToken token = default)
        {
            var request = interactive.Type switch
            {
                "userSelect" => CreateUserSelectRequest(interactive, message, chatId, detail),
                "userInput" => CreateUserInputRequest(interactive, form, chatId, detail),
                _ => throw new NotSupportedException($"不支持的交互类型: {interactive.Type}")
            };
            return ChatStreamAsync(appName, request, token);
        }

        /// <summary>
        /// 创建用户选择类型的请求
        /// </summary>
        /// <param name="interactive">交互对象</param>
        /// <param name="message">用户选择的消息</param>
        /// <param name="chatId">对话ID</param>
        /// <param name="detail">是否返回中间值</param>
        /// <returns></returns>
        private static ChatStreamRequest CreateUserSelectRequest(Interactive interactive, string? message, string chatId, bool detail)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message), "请选择一个选项");

            var validOptions = interactive.Params?.UserSelectOptions?.Select(o => o.Value) ?? [];
            if (!validOptions.Contains(message))
                throw new ArgumentOutOfRangeException(nameof(message), message, "不在可选项范围内");

            return new ChatStreamRequest([new ChatBaseMessage(message)], chatId, detail);
        }

        /// <summary>
        /// 创建用户输入类型的请求
        /// </summary>
        /// <param name="interactive">交互对象</param>
        /// <param name="form">表单数据</param>
        /// <param name="chatId">对话ID</param>
        /// <param name="detail">是否返回中间值</param>
        /// <returns></returns>
        private static ChatStreamRequest CreateUserInputRequest(Interactive interactive, Dictionary<string, object> form, string chatId, bool detail)
        {
            if (form is not { Count: > 0 })
                throw new ArgumentException("表单输入不能为空", nameof(form));

            var inputForm = interactive.Params?.InputForm;
            if (inputForm is not { Length: > 0 })
                throw new InvalidOperationException("非法的interactive，Type为userInput时InputForm不能为空");

            ValidateFormFields(form, inputForm);

            return new ChatStreamRequest([new ChatBaseMessage(JsonSerializer.Serialize(form, FastGptJsonOptions.Options))], chatId, detail);
        }

        /// <summary>
        /// 验证表单字段
        /// </summary>
        /// <param name="form">表单数据</param>
        /// <param name="inputForm">表单定义</param>
        private static void ValidateFormFields(Dictionary<string, object> form, InputFormItem[] inputForm)
        {
            var inputFormKeys = inputForm.Select(f => f.Key).ToHashSet();
            var formKeys = form.Keys.ToHashSet();
            var requiredKeys = inputForm.Where(f => f.Required).Select(f => f.Key).ToHashSet();

            var invalidKeys = formKeys.Except(inputFormKeys);
            if (invalidKeys.Any())
                throw new ArgumentException($"表单包含无效字段: {string.Join(", ", invalidKeys)}", nameof(form));

            var missingKeys = requiredKeys.Except(formKeys);
            if (missingKeys.Any())
                throw new ArgumentException($"缺少必填字段: {string.Join(", ", missingKeys)}", nameof(form));
        }

        /// <summary>
        /// 图片消息对话
        /// </summary>
        /// <param name="imageUrl">图片URL</param>
        /// <param name="chatId">会话ID</param>
        /// <param name="detail">是否返回中间值</param>
        /// <param name="token">取消令牌</param>
        /// <returns></returns>
        public Task<ChatResponse> ChatWithImageAsync(string appName, string imageUrl, string? chatId = null, bool detail = true, CancellationToken token = default) =>
            chatApi.ChatAsync(appName, new ChatNoneStreamRequest([new ChatContentMessage([ContentItem.FromImage(imageUrl)])], chatId, detail), token);

        /// <summary>
        /// 文件消息对话
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileUrl">文件URL</param>
        /// <param name="chatId">会话ID</param>
        /// <param name="detail">是否返回中间值</param>
        /// <param name="token">取消令牌</param>
        /// <returns></returns>
        public Task<ChatResponse> ChatWithFileAsync(string appName, string fileName, string fileUrl, string? chatId = null, bool detail = true, CancellationToken token = default) =>
            chatApi.ChatAsync(appName, new ChatNoneStreamRequest([new ChatContentMessage([ContentItem.FromFile(fileName, fileUrl)])], chatId, detail), token);

        /// <summary>
        /// 交互式对话
        /// </summary>
        /// <param name="interactive">交互对象</param>
        /// <param name="chatId">会话ID</param>
        /// <param name="message">用户选择的消息（userSelect类型使用）</param>
        /// <param name="form">表单数据（userInput类型使用）</param>
        /// <param name="detail">是否返回中间值</param>
        /// <param name="token">取消令牌</param>
        /// <returns></returns>
        public Task<ChatResponse> ChatInteractiveAsync(string appName, Interactive interactive, string chatId, string? message, Dictionary<string, object> form, bool detail = true, CancellationToken token = default)
        {
            var request = interactive.Type switch
            {
                "userSelect" => CreateUserSelectRequest(interactive, message, chatId, detail),
                "userInput" => CreateUserInputRequest(interactive, form, chatId, detail),
                _ => throw new NotSupportedException($"不支持的交互类型: {interactive.Type}")
            };
            return chatApi.ChatAsync(appName, new ChatNoneStreamRequest(request.Messages, request.ChatId, request.Detail, request.Variables, request.ResponseChatItemId), token);
        }

        /// <summary>
        /// 请求插件 非流式响应
        /// 具体文档请见<seealso href="https://doc.fastgpt.cn/docs/introduction/development/openapi/chat#%E5%93%8D%E5%BA%94%E7%A4%BA%E4%BE%8B">响应示例</seealso>
        /// </summary>
        /// <param name="variables">插件的输入</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<ChatResponse> RequestPluginAsync(string appName, Dictionary<string, object> variables, CancellationToken token = default) =>
            chatApi.ChatAsync(appName, new ChatNoneStreamRequest(null, Variables: variables), token);

        /// <summary>
        /// 请求插件 流式响应
        /// 具体文档请见<seealso href="https://doc.fastgpt.cn/docs/introduction/development/openapi/chat#%E5%93%8D%E5%BA%94%E7%A4%BA%E4%BE%8B">响应示例</seealso>中detail=true,stream=true的tab
        /// </summary>
        /// <param name="variables">插件的输入</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public IAsyncEnumerable<SseItem<object?>> RequestPluginStreamAsync(string appName, Dictionary<string, object> variables, CancellationToken token = default)
            => ChatStreamAsync(appName, new ChatStreamRequest(null, Variables: variables), token);

        /// <summary>
        /// 获取会话历史列表
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="offset">分页偏移量</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="source">来源 值为api，表示获取通过 API 创建的对话（不会获取到页面上的对话记录）</param>
        /// <param name="token">取消令牌</param>
        /// <returns></returns>
        public Task<ChatHistoryResponse> GetHistoriesAsync(string appName, int offset, int pageSize, string? source = null, CancellationToken token = default) =>
            chatApi.GetHistoriesAsync(appName, new ChatHistoryRequest 
            {
                Offset = offset,
                PageSize = pageSize,
                Source = source
            }, token);

        /// <summary>
        /// 更新会话标题
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="chatId">会话id</param>
        /// <param name="title">会话标题</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task UpdateHistoryAsync(string appName, string chatId, string title, CancellationToken token = default)
        {
            return chatApi.UpdateHistoryAsync(appName, new UpdateChatHistoryRequest(chatId, title), token);
        }
            
    }
}
