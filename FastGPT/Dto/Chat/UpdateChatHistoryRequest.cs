namespace FastGPT.Dto.Chat
{
    /// <summary>
    /// 更新会话标题
    /// </summary>
    /// <param name="ChatId">会话id</param>
    /// <param name="Title">标题</param>
    public record UpdateChatHistoryRequest(string ChatId, string Title) : IAppId
    {
        /// <summary>
        /// 应用id
        /// </summary>
        public string AppId { get; set; } = string.Empty;
    }
}
