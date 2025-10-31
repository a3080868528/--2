// Program.cs
using GameServer.Services.Battle; // ����ս������
using GameServer.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// ע��MongoDB�����䣩
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = new MongoClient("mongodb://localhost:27017");
    return client.GetDatabase("GameDB");
});

// ע��WebSocket���񣨲��䣩
builder.Services.AddWebSockets(_ => { });

// ע��ս�����񣨽ӿ�+ʵ�֣�
builder.Services.AddScoped<IBattleService, BattleService>();
// ע��WebSocketHandler����Ϊ����Ҫ����IBattleService��
builder.Services.AddScoped<WebSocketHandler>();

var app = builder.Build();

// ����WebSocket�����䣩
app.UseWebSockets();

// ӳ��WebSocket·��������Ҫ�ģ���ΪWebSocketHandler��������ͨ�࣬��Ҫͨ������ע���ȡ��
app.Map("/ws", async context =>
{
    // �������л�ȡWebSocketHandlerʵ�����Զ�ע��IBattleService��
    var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
    await handler.HandleWebSocketAsync(context, _ => Task.CompletedTask);
});

app.Run("http://localhost:5000");