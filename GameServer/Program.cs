using GameServer.Services.Battle;
using GameServer.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// �������ļ���ȡMongoDB������Ϣ
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
                           ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"]
                        ?? "GameDB";

// ע��MongoDB
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = new MongoClient(mongoConnectionString);
    return client.GetDatabase(mongoDatabaseName);
});

// ����WebSocket
builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
});

// ע�����
builder.Services.AddScoped<IBattleService, BattleService>();
builder.Services.AddScoped<WebSocketHandler>();

var app = builder.Build();

// ��������������ϸ������Ϣ
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// ����WebSocket�м��
app.UseWebSockets();

// ӳ��WebSocket�˵�
app.Map("/ws", async context =>
{
    var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
    await handler.HandleWebSocketAsync(context, context.RequestServices.GetRequiredService<RequestDelegate>());
});

// ����������
app.Run("http://localhost:5000");