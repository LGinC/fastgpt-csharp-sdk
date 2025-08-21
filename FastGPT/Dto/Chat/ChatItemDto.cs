namespace FastGPT.Dto.Chat
{
    /// <summary>
    /// 会话信息
    /// </summary>
    public sealed class ChatItemDto
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public required string ChatId { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTimeOffset? UpdateTime { get; set; }
        
        /// <summary>
        /// 应用ID
        /// </summary>
        public required string AppId { get; set; }
        
        /// <summary>
        /// 自定义标题
        /// </summary>
        public string? CustomTitle { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        public required string Title { get; set; }
        
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool Top { get; set; }
    }

    /// <summary>
    /// 指定应用的会话历史
    /// </summary>
    public sealed class ChatHistoryResponse : FastGPTResponse<FastGPTListResponse<ChatItemDto>>
    {
    }

    /// <summary>
    /// 聊天历史请求
    /// </summary>
    public sealed class ChatHistoryRequest :  IAppId
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; } = string.Empty;
        
        /// <summary>
        /// 偏移量
        /// </summary>
        public int Offset { get; set; }
        
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 来源 值为api，表示获取通过 API 创建的对话（不会获取到页面上的对话记录）
        /// </summary>
        public string? Source { get; set; }
    }
}
