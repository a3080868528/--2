// WebSockets/WebSocketHandler.cs
using GameServer.Models;
using GameServer.Models.Messages;
using GameServer.Services.Battle; // ����ս������ӿ�
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace GameServer.WebSockets;

// ȥ��static���ĳ���ͨ��
public class WebSocketHandler
{
    private readonly IBattleService _battleService; // ͨ��ע���ȡ
    private readonly Dictionary<string, WebSocket> _connections = new();

    // ���캯����ע��IBattleService
    public WebSocketHandler(IBattleService battleService)
    {
        _battleService = battleService; // ��ֵ
    }

    // ԭ����HandleWebSocketAsync����ȥ��static����������
    public async Task HandleWebSocketAsync(HttpContext context, RequestDelegate next)
    {
        // ���ݺ�ԭ��һ����ֻ��ȥ��static
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        string userId = Guid.NewGuid().ToString();
        _connections.Add(userId, webSocket);
        Console.WriteLine($"�û� {userId} ����WebSocket");

        try
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string messageText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await HandleMessageAsync(userId, messageText);
                }
            } while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        finally
        {
            _connections.Remove(userId);
            Console.WriteLine($"�û� {userId} �Ͽ�WebSocket");
        }
    }

    // HandleMessageAsyncȥ��static��ʹ��ע���_battleService
    private async Task HandleMessageAsync(string userId, string messageText)
    {
        var message = JsonSerializer.Deserialize<Message>(messageText);
        if (message == null) return;

        switch (message.Type)
        {
            case "attack":
                var attackRequest = JsonSerializer.Deserialize<AttackRequest>(message.Data);
                // ���ﲻ��new��������ע���_battleService
                var attackResult = _battleService.CalculateDamage(attackRequest);
                await SendMessageAsync(userId, "attack_result", attackResult);
                break;
        }
    }

    // �޸ķ������壬���� messageType ����
    public async Task SendMessageAsync(string userId, string messageType, object data)
    {
        if (_connections.TryGetValue(userId, out var webSocket) && webSocket.State == WebSocketState.Open)
        {
            // �����������ͺ����ݵ� Message ����
            var message = new Message
            {
                Type = messageType,
                Data = data
            };
            var json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}