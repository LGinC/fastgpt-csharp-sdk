## 使用
假设FastGPT的服务器是192.168.1.10，监听的端口是3000,则执行以下命令设置,记得将appId和apikey替换为实际的
```powershell
dotnet user-secrets set FastGPT:Host http://192.168.1.10:3000
dotnet user-secrets set FastGPT:AppApiKeys:测试:AppId 6896128736578
dotnet user-secrets set FastGPT:AppApiKeys:测试:AppKey fastgpt-tihmPssnwiuYnwh
```