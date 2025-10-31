using GameServer.Services.Battle;
using GameServer.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 从配置文件读取MongoDB连接信息
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
                           ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"]
                        ?? "GameDB";

// 注册MongoDB
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = new MongoClient(mongoConnectionString);
    return client.GetDatabase(mongoDatabaseName);
});

// 配置WebSocket
builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
});

// 注册服务
builder.Services.AddScoped<IBattleService, BattleService>();
builder.Services.AddScoped<WebSocketHandler>();

var app = builder.Build();

// 开发环境启用详细错误信息
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 启用WebSocket中间件
app.UseWebSockets();

// 映射WebSocket端点
app.Map("/ws", async context =>
{
    var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
    await handler.HandleWebSocketAsync(context, context.RequestServices.GetRequiredService<RequestDelegate>());
});

// 启动服务器
app.Run("http://localhost:5000");