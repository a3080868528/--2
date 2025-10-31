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
        PropertyNameCaseInsensitive = true
    };
    // �̰߳�ȫ������
    private readonly object _lockObj = new();

    public WebSocketHandler(IBattleService battleService)
    {
        _battleService = battleService;
    }

    public async Task HandleWebSocketAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var userId = Guid.NewGuid().ToString();

        // �̰߳�ȫ�������
        lock (_lockObj)
        {
            _connections.Add(userId, webSocket);
        }
        Console.WriteLine($"�û� {userId} ���ӣ���ǰ����: {_connections.Count} ��");

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
                    await HandleClientMessageAsync(userId, messageText);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "�ͻ��������ر�",
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
            // �̰߳�ȫ�Ƴ�����
            lock (_lockObj)
            {
                _connections.Remove(userId);
            }
            Console.WriteLine($"�û� {userId} �Ͽ�����ǰ����: {_connections.Count} ��");
        }
    }

    private async Task HandleClientMessageAsync(string userId, string messageText)
    {
        try
        {
            var message = JsonSerializer.Deserialize<Message>(messageText, _jsonOptions);
            if (message == null)
            {
                await SendMessageToUserAsync(userId, "error", "��Ϣ��ʽ��Ч");
                return;
            }

            switch (message.Type)
            {
                case "attack":
                    await HandleAttackMessage(userId, message.Data);
                    break;
                default:
                    await SendMessageToUserAsync(userId, "error", $"��֧�ֵ���Ϣ����: {message.Type}");
                    break;
            }
        }
        catch (JsonException)
        {
            await SendMessageToUserAsync(userId, "error", "��Ϣ����ʧ��");
        }
    }

    private async Task HandleAttackMessage(string userId, string attackData)
    {
        var attackRequest = JsonSerializer.Deserialize<AttackRequest>(attackData, _jsonOptions);
        if (attackRequest == null)
        {
            await SendMessageToUserAsync(userId, "error", "�������ݸ�ʽ��Ч");
            return;
        }

        var attackResult = _battleService.CalculateDamage(attackRequest);
        await SendMessageToUserAsync(userId, "attack_result", attackResult);
    }

    public async Task SendMessageToUserAsync(string userId, string messageType, object data)
    {
        WebSocket? webSocket = null;
        // �̰߳�ȫ��ȡ����
        lock (_lockObj)
        {
            _connections.TryGetValue(userId, out webSocket);
        }

        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            Console.WriteLine($"�û� {userId} �����ߣ���Ϣ����ʧ��");
            return;
        }

        try
        {
            var message = new Message
            {
                Type = messageType,
                Data = JsonSerializer.Serialize(data, _jsonOptions)
            };
            var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, _jsonOptions));

            await webSocket.SendAsync(
                new ArraySegment<byte>(jsonBytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"���û� {userId} ������Ϣʧ��: {ex.Message}");
        }
    }
}