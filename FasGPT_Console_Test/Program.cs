using FastGPT;
using FastGPT.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApiClientCore.Extensions.OAuths;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((context, services) =>
{
    services.AddTokenProvider<IFastGPTApi>(s =>
     {
         //var apiKey = s.GetRequiredService<DifyAIOptions>().DefaultApiKey;
         var apiKey = context.Configuration["FastGPT_ApiKey"];
         return Task.FromResult<TokenResult?>(new TokenResult { Access_token = apiKey });
     });
    services.AddHttpApi<IChatApi>()
    .ConfigureHttpApi(o =>
    {
        var host = context.Configuration["FastGPT_Host"];
        o.HttpHost = new Uri($"{context.Configuration["FastGPT_Host"]}/api/");
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
});

var app = builder.Build();

// app.Run() is a blocking call, so code after it will not execute until the application shuts down.
// To use the services, you should get them from the host's service provider before the application runs,
// or run your code as a hosted service.

// Here's how you can get IChatApi and call a method on it:
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var chatService = services.GetRequiredService<ChatService>();
        
        // 测试生成会话ID
        var chatId = ChatService.GenerateChatId();
        Console.WriteLine($"Generated Chat ID: {chatId}");
        
        // 测试文本消息流式对话
        Console.WriteLine("\n=== 测试文本消息流式对话 ===");
        await foreach (var item in chatService.ChatStreamAsync("你好，请介绍一下自己", chatId))
        {
            Console.WriteLine($"Event: {item.EventType}, Data: {JsonSerializer.Serialize(item.Data, FastGptJsonOptions.Options)}");
        }
        
        // 测试图片消息流式对话
        Console.WriteLine("\n=== 测试图片消息流式对话 ===");
        await foreach (var item in chatService.ChatStreamWithImageAsync("https://example.com/image.jpg", chatId))
        {
            Console.WriteLine($"Event: {item.EventType}, Data: {JsonSerializer.Serialize(item.Data, FastGptJsonOptions.Options)}");
        }
        
        // 测试文件消息流式对话
        Console.WriteLine("\n=== 测试文件消息流式对话 ===");
        await foreach (var item in chatService.ChatStreamWithFileAsync("test.pdf", "https://example.com/test.pdf", chatId))
        {
            Console.WriteLine($"Event: {item.EventType}, Data: {JsonSerializer.Serialize(item.Data, FastGptJsonOptions.Options)}");
        }
        
        // 测试交互式对话 - userSelect类型
        Console.WriteLine("\n=== 测试交互式对话 - userSelect ===");
        var selectInteractive = new Interactive
        {
            Type = "userSelect",
            Params = new InteractiveParams
            {
                Description = "请选择一个选项",
                UserSelectOptions = [
                    new UserSelectOption { Key = "option1", Value = "选项1" },
                    new UserSelectOption { Key = "option2", Value = "选项2" }
                ]
            }
        };
        await foreach (var item in chatService.ChatStreamInteractiveAsync(selectInteractive, chatId, "选项1", new Dictionary<string, object>()))
        {
            Console.WriteLine($"Event: {item.EventType}, Data: {JsonSerializer.Serialize(item.Data, FastGptJsonOptions.Options)}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

// If you need the application to continue running for background tasks, you can use app.Run() or await app.RunAsync().
// If you just want to execute the API call and exit, you can remove the app.Run() call.
app.Run();

