using FastGPT.Dto;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace FastGPT;

[OAuthToken]
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
    /// <param name="request">流式对话请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    [HttpPost("v1/chat/completions")]
    Task<Stream> ChatAsync([JsonContent] ChatStreamRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送普通对话请求（非流式）
    /// <seealso href="https://doc.fastgpt.cn/docs/introduction/development/openapi/chat#%E5%93%8D%E5%BA%94">对话接口</seealso>
    /// </summary>
    /// <param name="request">普通对话请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    [HttpPost("v1/chat/completions")]
    Task<ChatResponse> ChatAsync([JsonContent] ChatNoneStreamRequest request, CancellationToken cancellationToken = default);
}
