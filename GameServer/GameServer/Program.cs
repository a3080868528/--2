using GameServer.Services.Battle;
using GameServer.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// MongoDB 配置
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
                           ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"]
                        ?? "GameDB";

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = new MongoClient(mongoConnectionString);
    return client.GetDatabase(mongoDatabaseName);
});

// WebSocket 配置
builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
});

// 服务注册（修正生命周期为 Singleton）
builder.Services.AddSingleton<IBattleService, BattleService>();
builder.Services.AddSingleton<WebSocketHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseWebSockets();

app.Map("/ws", async (HttpContext context, RequestDelegate next) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
    await webSocketHandler.HandleWebSocketAsync(context);
});

app.Run("http://localhost:5000");