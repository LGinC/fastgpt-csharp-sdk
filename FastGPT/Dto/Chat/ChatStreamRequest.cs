using System.ComponentModel;

namespace FastGPT.Dto.Chat
{
    /// <summary>
    /// 流式对话请求
    /// </summary>
    /// <param name="Messages">消息列表</param>
    /// <param name="ChatId">对话id，为空时不使用 FastGpt 提供的上下文功能，完全通过传入的 messages 构建上下文
    /// <para>不为空时意味着使用 chatId 进行对话，自动从 FastGpt 数据库取历史记录，并使用 messages数组最后一个内容作为用户问题，其余 message 会被忽略。请自行确保 chatId唯一，长度小于250，通常可以是自己系统的对话框ID</para> </param>
    /// <param name="Detail">是否返回中间值 （模块状态，响应的完整结果等）,stream模式下会通过event进行区分</param>
    /// <param name="ResponseChatItemId">如果传入，则会将该值作为本次对话的响应消息的 ID，FastGPT会自动将该 ID 存入数据库。请确保，在当前chatId下，responseChatItemId是唯一的</param>
    /// <param name="Variables">模块变量，会替换模块中，输入框内容里的[key]</param>
    [Description("流式对话请求")]
    public record ChatStreamRequest(
        [property: Description("消息列表")] List<ChatMessage>? Messages,
        [property: Description("对话id")] string? ChatId = null,
        [property: Description("是否返回中间值")] bool Detail = false,
        [property: Description("变量列表")] Dictionary<string, object>? Variables = null,
        [property: Description("响应id")] string? ResponseChatItemId = null
        )
    {
        public virtual bool Stream => true;
    }

    /// <summary>
    /// 非流式对话请求
    /// </summary>
    /// <param name="Messages">消息列表, 当为插件请求时不传</param>
    /// <param name="ChatId">对话id，为空时不使用 FastGpt 提供的上下文功能，完全通过传入的 messages 构建上下文
    /// <para>不为空时意味着使用 chatId 进行对话，自动从 FastGpt 数据库取历史记录，并使用 messages数组最后一个内容作为用户问题，其余 message 会被忽略。请自行确保 chatId唯一，长度小于250，通常可以是自己系统的对话框ID</para> </param>
    /// <param name="Detail">是否返回中间值 （模块状态，响应的完整结果等）,stream模式下会通过event进行区分</param>
    /// <param name="ResponseChatItemId">如果传入，则会将该值作为本次对话的响应消息的 ID，FastGPT会自动将该 ID 存入数据库。请确保，在当前chatId下，responseChatItemId是唯一的</param>
    /// <param name="Variables">模块变量，会替换模块中，输入框内容里的[key]</param>
    public sealed record ChatNoneStreamRequest(
        List<ChatMessage>? Messages,
        string? ChatId = null,
        bool Detail = false,
        Dictionary<string, object>? Variables = null,
        string? ResponseChatItemId = null) : ChatStreamRequest(Messages, ChatId, Detail, Variables, ResponseChatItemId)
    {
        public override bool Stream => false;
    }

    /// <summary>
    /// 基础消息
    /// </summary>
    /// <param name="Role">角色</param>
    public abstract record ChatMessage(string Role);

    /// <summary>
    /// 基础文本消息
    /// </summary>
    /// <param name="Role">角色</param>
    /// <param name="Content">内容</param>
    public record ChatBaseMessage(string Content, string Role = "user") : ChatMessage(Role);

    /// <summary>
    /// 复杂内容消息
    /// </summary>
    /// <param name="Role">角色</param>
    /// <param name="Content">内容</param>
    public record ChatContentMessage(ContentItem[] Content, string Role = "user" ) : ChatMessage(Role);

    /// <summary>
    /// 复杂内容
    /// </summary>
    public record ContentItem
    {
        /// <summary>
        /// 内容类型
        /// </summary>
        public string Type { get; init; } = string.Empty;
        /// <summary>
        /// 文本内容
        /// </summary>
        public string? Text { get; init; }
        /// <summary>
        /// 图片链接
        /// </summary>
        public ImageUrl? Image_url { get; init; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string? Name { get; init; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string? Url { get; init; }

        /// <summary>
        /// 创建文本内容
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static ContentItem FromText(string text) =>
            new() { Type = "text", Text = text };

        /// <summary>
        /// 创建图片消息
        /// </summary>
        /// <param name="imageUrl">图片url</param>
        /// <returns></returns>
        public static ContentItem FromImage(string imageUrl) =>
            new()
            {
                Type = "image_url",
                Image_url = new ImageUrl(imageUrl),
            };

        /// <summary>
        /// 创建文件消息
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileUrl">文件url</param>
        /// <returns></returns>
        public static ContentItem FromFile(string fileName, string fileUrl) =>
            new()
            {
                Type = "file_url",
                Name = fileName,
                Url = fileUrl,
            };
    }

    /// <summary>
    /// 图片url
    /// </summary>
    /// <param name="Url">url</param>
    public record ImageUrl(string Url);

    /// <summary>
    /// 流式对话响应事件
    /// </summary>
    public static class ChatEventConsts
    {
        /// <summary>
        /// 当detail=false时返回的eventType
        /// </summary>
        public const string Message = "message";
        /// <summary>
        /// 返回给客户端的文本（最终会算作回答）
        /// </summary>
        public const string Answer = "answer";
        /// <summary>
        /// 指定回复返回给客户端的文本（最终会算作回答） data就为string
        /// </summary>
        public const string FastAnswer = "fastAnswer";
        /// <summary>
        /// 执行工具
        /// </summary>
        public const string ToolCall = "toolCall";
        /// <summary>
        /// 工具参数
        /// </summary>
        public const string ToolParams = "toolParams";
        /// <summary>
        /// 工具返回
        /// </summary>
        public const string ToolResponse = "toolResponse";
        /// <summary>
        /// 运行到的节点状态
        /// </summary>
        public const string FlowNodeStatus = "flowNodeStatus";
        /// <summary>
        /// 节点完整响应
        /// </summary>
        public const string FlowResponses = "flowResponses";

        /// <summary>
        /// 交互节点，需要再次调用发起对话接口，将输入或选择传入到message中，并传入本次的chatId
        /// </summary>
        public const string Interactive = "interactive";
        /// <summary>
        /// 更新变量
        /// </summary>
        public const string UpdateVariables = "updateVariables";
        /// <summary>
        /// 报错
        /// </summary>
        public const string Error = "Error";
    }
}
