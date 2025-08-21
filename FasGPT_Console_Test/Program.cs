using FastGPT;
using FastGPT.Dto;
using FastGPT.Dto.Chat;
using FastGPT.Options;
using FastGPT.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

IHostBuilder builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((context, services) =>
{
    services.AddFastGPT(context.Configuration);
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
        var appName = "测试";

        var historyResult = await chatService.GetHistoriesAsync(appName, 0, 20);
        await chatService.UpdateHistoryAsync(appName, historyResult.Data!.List[0].ChatId, "自定义标题");


        return;

        // 测试文本消息流式对话
        Console.WriteLine("\n=== 测试文本消息流式对话 ===");
        await foreach (var item in chatService.ChatStreamAsync(appName, "你好，请介绍一下自己", chatId))
        {
            Console.WriteLine($"Event: {item.EventType}, Data: {JsonSerializer.Serialize(item.Data, FastGptJsonOptions.Options)}");
        }
        
        // 测试图片消息流式对话
        Console.WriteLine("\n=== 测试图片消息流式对话 ===");
        await foreach (var item in chatService.ChatStreamWithImageAsync(appName, "https://example.com/image.jpg", chatId))
        {
            Console.WriteLine($"Event: {item.EventType}, Data: {JsonSerializer.Serialize(item.Data, FastGptJsonOptions.Options)}");
        }
        
        // 测试文件消息流式对话
        Console.WriteLine("\n=== 测试文件消息流式对话 ===");
        await foreach (var item in chatService.ChatStreamWithFileAsync(appName, "test.pdf", "https://example.com/test.pdf", chatId))
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
        await foreach (var item in chatService.ChatStreamInteractiveAsync(appName, selectInteractive, chatId, "选项1", new Dictionary<string, object>()))
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

