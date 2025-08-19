using System.ComponentModel;

namespace FastGPT.Dto
{
    /// <summary>
    /// 聊天响应
    /// </summary>
    [Description("聊天响应")]
    public record ChatResponse
    {
        /// <summary>响应ID</summary>
        public string Id { get; init; } = string.Empty;
        /// <summary>模型名称</summary>
        public string Model { get; init; } = string.Empty;
        /// <summary>令牌使用统计</summary>
        public Usage? Usage { get; init; }
        /// <summary>响应选择列表</summary>
        public Choice[]? Choices { get; init; }
        /// <summary>FastGPT模块响应数据</summary>
        public ResponseData[]? ResponseData { get; init; }
    }

    /// <summary>
    /// 令牌使用统计
    /// </summary>
    /// <param name="Prompt_tokens">提示令牌数</param>
    /// <param name="Completion_tokens">完成令牌数</param>
    /// <param name="Total_tokens">总令牌数</param>
    public record Usage(int Prompt_tokens, int Completion_tokens, int Total_tokens);

    /// <summary>
    /// 响应选择项
    /// </summary>
    public record Choice
    {
        /// <summary>消息内容</summary>
        public Message? Message { get; init; }
        /// <summary>完成原因</summary>
        public string Finish_reason { get; init; } = string.Empty;
        /// <summary>选择索引</summary>
        public int Index { get; init; }
    }

    /// <summary>
    /// 消息
    /// </summary>
    /// <param name="Role">角色</param>
    /// <param name="Content">内容</param>
    public record Message(string Role, string Content);

    /// <summary>
    /// FastGPT模块响应数据
    /// </summary>
    public record ResponseData
    {
        /// <summary>模块名称</summary>
        public string ModuleName { get; init; } = string.Empty;
        /// <summary>价格</summary>
        public double Price { get; init; }
        /// <summary>模型名称</summary>
        public string Model { get; init; } = string.Empty;
        /// <summary>令牌数</summary>
        public int Tokens { get; init; }
        /// <summary>相似度</summary>
        public double? Similarity { get; init; }
        /// <summary>限制数量</summary>
        public int? Limit { get; init; }
        /// <summary>问题</summary>
        public string? Question { get; init; }
        /// <summary>答案</summary>
        public string? Answer { get; init; }
        /// <summary>最大令牌数</summary>
        public int? MaxToken { get; init; }
        /// <summary>引用列表</summary>
        public QuoteItem[]? QuoteList { get; init; }
        /// <summary>完整消息列表</summary>
        public CompleteMessage[]? CompleteMessages { get; init; }
    }

    /// <summary>
    /// 引用项
    /// </summary>
    public record QuoteItem
    {
        /// <summary>数据集ID</summary>
        public string Dataset_id { get; init; } = string.Empty;
        /// <summary>项目ID</summary>
        public string Id { get; init; } = string.Empty;
        /// <summary>问题</summary>
        public string Q { get; init; } = string.Empty;
        /// <summary>答案</summary>
        public string A { get; init; } = string.Empty;
        /// <summary>来源</summary>
        public string Source { get; init; } = string.Empty;
    }

    /// <summary>
    /// 完整消息
    /// </summary>
    /// <param name="Obj">对象类型</param>
    /// <param name="Value">消息值</param>
    public record CompleteMessage(string Obj, string Value);
}