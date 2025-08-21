using FastGPT.Dto;
using FastGPT.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace FastGPT.Filters
{
    /// <summary>
    /// 根据appName设置Authorization请求头和appId参数值
    /// </summary>
    public sealed class AppNameTokenFilter : ApiFilterAttribute
    {
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var options = context.HttpContext.ServiceProvider.GetRequiredService<IOptionsSnapshot<FastGPTOptions>>().Value;
            ArgumentNullException.ThrowIfNull(options);
            
            // 设置Authorization
            if (!context.TryGetArgument<string>("appName", out var appName))
            {
                ArgumentNullException.ThrowIfNull(options.GlobalApiKey);
                SetAuthorization(context, options.GlobalApiKey);
                return Task.CompletedTask;
            }

            if (options.AppApiKeys?.TryGetValue(appName, out var appInfo) is not true)
                throw new InvalidOperationException($"找不到 appName={appName} 的配置");

            SetAuthorization(context, appInfo.ApiKey);
            SetAppId(context, appInfo.AppId, appName);
            
            return Task.CompletedTask;
        }

        private static void SetAuthorization(ApiRequestContext context, string apiKey)
        {
            context.HttpContext.RequestMessage.Headers.Authorization = new("Bearer", apiKey);
        }

        private static void SetAppId(ApiRequestContext context, string appId, string appName)
        {
            if (string.IsNullOrEmpty(appId))
                throw new InvalidOperationException($"找不到 appName={appName} 的 AppId");

            // 优先设置直接的appId参数
            var appIdParam = context.ActionDescriptor.Parameters.FirstOrDefault(p => p.Name == "appId");
            if (appIdParam is not null)
            {
                context.Arguments[appIdParam.Index] = appId;
                return;
            }

            // 设置实现IAppId接口的参数
            var appIdInterfaceParam = context.ActionDescriptor.Parameters.FirstOrDefault(p => typeof(IAppId).IsAssignableFrom(p.ParameterType));
            if (appIdInterfaceParam is not null && context.Arguments[appIdInterfaceParam.Index] is IAppId para)
            {
                para.AppId = appId;
            }
        }

        public override Task OnResponseAsync(ApiResponseContext context) => Task.CompletedTask;
    }
}
