using GameServer.Models;
using GameServer.Models.Messages;
using GameServer.Services.Battle;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace GameServer.WebSockets;

public class WebSocketHandler
{
    private readonly IBattleService _battleService;
    private readonly Dictionary<string, WebSocket> _connections = new();
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public WebSocketHandler(IBattleService battleService)
    {
        _battleService = battleService;
    }

    public async Task HandleWebSocketAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var userId = Guid.NewGuid().ToString();

        lock (_connections)
        {
            _connections.Add(userId, webSocket);
        }

        Console.WriteLine($"�û� {userId} �����ӣ���ǰ������: {_connections.Count}");

        try
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await HandleMessageAsync(userId, messageText);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "�����ر�",
                        CancellationToken.None);
                }

            } while (!result.CloseStatus.HasValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket����: {ex.Message}");
        }
        finally
        {
            lock (_connections)
            {
                _connections.Remove(userId);
            }
            Console.WriteLine($"�û� {userId} �ѶϿ�����ǰ������: {_connections.Count}");
        }
    }

    private async Task HandleMessageAsync(string userId, string messageText)
    {
        try
        {
            var message = JsonSerializer.Deserialize<Message>(messageText, _jsonOptions);
            if (message == null)
            {
                await SendErrorMessageAsync(userId, "��Ч����Ϣ��ʽ");
                return;
            }

            switch (message.Type)
            {
                case "attack":
                    await HandleAttackMessage(userId, message.Data);
                    break;
                case "login":
                    await SendMessageAsync(userId, "login_ack", new { Success = true, UserId = userId });
                    break;
                default:
                    await SendErrorMessageAsync(userId, $"��֧�ֵ���Ϣ����: {message.Type}");
                    break;
            }
        }
        catch (JsonException)
        {
            await SendErrorMessageAsync(userId, "��Ϣ����ʧ��");
        }
        catch (Exception ex)
        {
            await SendErrorMessageAsync(userId, $"������Ϣʱ����: {ex.Message}");
        }
    }

    private async Task HandleAttackMessage(string userId, string data)
    {
        var attackRequest = JsonSerializer.Deserialize<AttackRequest>(data, _jsonOptions);
        if (attackRequest == null)
        {
            await SendErrorMessageAsync(userId, "���������ʽ����");
            return;
        }

        var attackResult = _battleService.CalculateDamage(attackRequest);
        await SendMessageAsync(userId, "attack_result", attackResult);
    }

    public async Task SendMessageAsync(string userId, string messageType, object data)
    {
        if (!_connections.TryGetValue(userId, out var webSocket) ||
            webSocket.State != WebSocketState.Open)
        {
            return;
        }

        try
        {
            var message = new Message
            {
                Type = messageType,
                Data = JsonSerializer.Serialize(data, _jsonOptions)
            };

            var json = JsonSerializer.Serialize(message, _jsonOptions);
            var buffer = Encoding.UTF8.GetBytes(json);

            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"������Ϣʧ��: {ex.Message}");
        }
    }

    private async Task SendErrorMessageAsync(string userId, string error)
    {
        await SendMessageAsync(userId, "error", new { Error = error });
    }
}