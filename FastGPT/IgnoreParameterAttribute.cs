using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace FastGPT
{
    /// <summary>
    /// 忽略Query参数
    /// </summary>
    public sealed class IgnoreParameterAttribute : PathQueryAttribute
    {
        public override Task OnRequestAsync(ApiParameterContext context) => Task.CompletedTask;
    }
}
