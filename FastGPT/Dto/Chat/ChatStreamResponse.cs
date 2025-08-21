namespace FastGPT.Dto.Chat
{
    /// <summary>
    /// 聊天流式响应数据传输对象
    /// </summary>
    public class ChatAnswerResponse
    {
        /// <summary>
        /// 响应ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 对象类型
        /// </summary>
        public string Object { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间戳
        /// </summary>
        public long Created { get; set; }

        /// <summary>
        /// 选择项列表
        /// </summary>
        public MessageChoice[] Choices { get; set; } = [];
    }

    /// <summary>
    /// 聊天流程节点状态响应
    /// </summary>
    public class ChatFlowNodeStatusResponse
    {
        /// <summary>
        /// 节点状态 'running' | 'finished' | 'error'
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 附加信息
        /// </summary>
        public string? Message { get; set; }
    }

    /// <summary>
    /// 流程节点完整响应
    /// </summary>
    public class ChatFlowResponse
    {

        #region 基础结构
        /// <summary>
        /// 节点ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>
        /// 模块类型
        /// </summary>
        public string ModuleType { get; set; } = string.Empty;

        /// <summary>
        /// 节点运行时间（秒）
        /// </summary>
        public double? RunningTime { get; set; }

        #endregion

        #region 通用字段

        /// <summary>
        /// 发给该节点的查询语句
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// 节点最终输出的文本
        /// </summary>
        public string? TextOutput { get; set; }
        /// <summary>
        /// 点运行时的错误对象或信息
        /// </summary>
        public object? Error { get; set; }
        /// <summary>
        /// 仅用于显示的错误文本
        /// </summary>
        public string? ErrorText { get; set; }
        /// <summary>
        /// 节点接收到的完整输入
        /// </summary>
        public object? NodeInputs { get; set; }
        /// <summary>
        /// 节点最终的完整输出
        /// </summary>
        public object? NodeOutputs { get; set; }

        /// <summary>
        /// 上下文总长度
        /// </summary>
        public int? ContextTotalLen { get; set; }

        #endregion

        #region 计费与模型信息
        /// <summary>
        /// 模型名称
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// 输入token数
        /// </summary>
        public int? InputTokens { get; set; }

        /// <summary>
        /// 输出token数
        /// </summary>
        public int? OutputTokens { get; set; }

        /// <summary>
        /// 本次节点运行消耗的总点数
        /// </summary>
        public int? TotalPoints { get; set; }
        #endregion

        #region “AI 对话”节点 (chatNode) 字段

        /// <summary>
        /// 在节点中配置的温度
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// 在节点中配置的最大token数
        /// </summary>
        public int? MaxToken { get; set; }

        /// <summary>引用列表</summary>
        public QuoteItem[]? QuoteList { get; set; }

        /// <summary>
        /// 模型的思维链文本
        /// </summary>
        public string? ReasoningText { get; set; }

        /// <summary>
        /// 发送给模型的最终上下文预览
        /// </summary>
        public CompleteMessage[]? HistoryPreview { get; set; }


        /// <summary>
        /// 结束原因 'stop', 'length' 等
        /// </summary>
        public string? FinishReason { get; set; }
        #endregion

        #region “知识库搜索”节点 (datasetSearchNode)

        /// <summary>
        /// 向量模型的名称
        /// </summary>
        public string? EmbeddingModel { get; set; }

        /// <summary>
        /// 向量token数
        /// </summary>
        public int? EmbeddingTokens { get; set; }

        /// <summary>
        /// 相似度
        /// </summary>
        public double? Similarity { get; set; }
    
        /// <summary>
        /// 搜索限制数量
        /// </summary>
        public int? Limit { get; set; }
   
        /// <summary>
        /// Rerank模型的名称
        /// </summary>
        public string? RerankModel { get; set; }
   
        /// <summary>
        /// 是否使用了Rerank
        /// </summary>
        public bool? SearchUsingReRank { get; set; }

        #endregion

        #region “HTTP 请求”节点

        /// <summary>
        /// Query Params
        /// </summary>
        public Dictionary<string, object>? Params { get; set; }
   
        /// <summary>
        /// Request Body
        /// </summary>
        public object? Body { get; set; }
   
        /// <summary>
        /// Request Headers
        /// </summary>
        public Dictionary<string, object>? Headers { get; set; }
   
        /// <summary>
        /// HTTP请求返回的结果
        /// </summary>
        public Dictionary<string, object>? HttpResult { get; set; }
        #endregion
    }

    /// <summary>
    /// 聊天交互响应
    /// </summary>
    public class ChatInteractiveResponse
    {
        /// <summary>
        /// 交互内容
        /// </summary>
        public Interactive Interactive { get; set; } = new();
    }

    /// <summary>
    /// 聊天工具调用响应
    /// </summary>
    public class ChatToolCallResponse
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// 工具名称
        /// </summary>
        public string ToolName { get; set; } = string.Empty;

        /// <summary>
        /// 工具头像
        /// </summary>
        public string ToolAvatar { get; set; } = string.Empty;
    }

    /// <summary>
    /// 聊天工具参数响应
    /// </summary>
    public class ChatToolParamsResponse
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// 参数JSON字符串
        /// </summary>
        public string Params { get; set; } = string.Empty;
    }

    /// <summary>
    /// 聊天工具响应
    /// </summary>
    public class ChatToolResponse
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// 响应JSON字符串
        /// </summary>
        public string Response { get; set; } = string.Empty;
    }

    /// <summary>
    /// 聊天错误响应
    /// </summary>
    public class ChatErrorResponse
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public ErrorInfo Error { get; set; } = new();
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 错误码
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }

    /// <summary>
    /// 交互对象
    /// </summary>
    public class Interactive
    {
        /// <summary>
        /// 交互类型 
        /// <para>为userSelect时使用Params.UserSelectOptions</para>
        /// <para>为userInput时使用Params.InputForm</para>
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 交互参数
        /// </summary>
        public InteractiveParams Params { get; set; } = new();
    }

    /// <summary>
    /// 交互参数
    /// </summary>
    public class InteractiveParams
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 用户选择选项
        /// </summary>
        public UserSelectOption[]? UserSelectOptions { get; set; }

        /// <summary>
        /// 输入表单
        /// </summary>
        public InputFormItem[]? InputForm { get; set; }
    }

    /// <summary>
    /// 用户选择选项
    /// </summary>
    public class UserSelectOption
    {
        /// <summary>
        /// 选项值
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 选项键
        /// </summary>
        public string Key { get; set; } = string.Empty;
    }

    /// <summary>
    /// 输入表单项
    /// </summary>
    public class InputFormItem
    {
        /// <summary>
        /// 输入类型
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 键名
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; } = string.Empty;

        /// <summary>
        /// 值类型
        /// </summary>
        public string ValueType { get; set; } = string.Empty;

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 选项列表
        /// </summary>
        public ListItem[]? List { get; set; }
    }

    /// <summary>
    /// 列表项
    /// </summary>
    public class ListItem
    {
        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// 选择项
    /// </summary>
    public class MessageChoice
    {
        /// <summary>
        /// 增量数据
        /// </summary>
        public MessageDelta Delta { get; set; } = new();

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 完成原因 当为 stop时表示正常结束
        /// </summary>
        public string? FinishReason { get; set; }
    }

    /// <summary>
    /// 增量数据
    /// </summary>
    public class MessageDelta
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 角色 assistant或者user
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
