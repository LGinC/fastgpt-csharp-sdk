using FastGPT.Dto;
using FastGPT.Dto.Chat;
using FastGPT.Filters;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace FastGPT.HttpApi;

[AppNameTokenFilter]
[LoggingFilter]
public interface IFastGPTApi : IHttpApi { }

/// <summary>
/// fastGPT对话api
/// </summary>
public interface IChatApi : IFastGPTApi
{
    /// <summary>
    /// 发送流式对话请求
    /// </summary>
    /// <param name="appName">仅用于通过appName查找对应的api key，不通过http发送到fastgpt服务端</param>
    /// <param name="request">流式对话请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    [HttpPost("v2s/chat/completions")]
    Task<Stream> ChatAsync([IgnoreParameter] string appName, [JsonContent] ChatStreamRequest request   , CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送普通对话请求（非流式）
    /// <seealso href="https://doc.fastgpt.cn/docs/introduction/development/openapi/chat#%E5%93%8D%E5%BA%94">对话接口</seealso>
    /// </summary>
    /// <param name="appName">仅用于通过appName查找对应的api key，不通过http发送到fastgpt服务端</param>
    /// <param name="request">普通对话请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    [HttpPost("v2/chat/completions")]
    Task<ChatResponse> ChatAsync([IgnoreParameter] string appName, [JsonContent] ChatNoneStreamRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会话记录列表
    /// </summary>
    /// <param name="appName">应用名称</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("core/chat/getHistories")]
    Task<ChatHistoryResponse> GetHistoriesAsync([IgnoreParameter] string appName, [JsonContent]ChatHistoryRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// 修改会话标题
    /// </summary>
    /// <param name="appName">应用名称</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("core/chat/updateHistory")]
    Task<FastGPTResponse> UpdateHistoryAsync([IgnoreParameter] string appName, [JsonContent] UpdateChatHistoryRequest request, CancellationToken cancellationToken = default);
}
