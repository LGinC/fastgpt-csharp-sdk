using FastGPT.Filters;
using FastGPT.HttpApi;
using FastGPT.Options;
using FastGPT.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FastGPT
{
    public static class FastGPTConfigExtension
    {
        public static void AddFastGPT(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FastGPTOptions>(configuration.GetSection("FastGPT"));
            services.AddHttpApi<IChatApi>()
            .ConfigureHttpApi((o, s) =>
            {
                using var sc = s.CreateScope();
                var options = sc.ServiceProvider.GetRequiredService<IOptionsSnapshot<FastGPTOptions>>();
                o.HttpHost = new Uri($"{options.Value.Host}/api/");
                o.UseLogging = true;
            });
            services.AddWebApiClient().ConfigureHttpApi(o =>
            {
                o.JsonSerializeOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.JsonDeserializeOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.KeyValueSerializeOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.JsonSerializeOptions.Converters.Add(new ChatMessageConverter());
                o.JsonSerializeOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            services.AddScoped<ChatService>();
        }
    }
}
