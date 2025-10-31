namespace GameServer.Models.Messages;

// 所有WebSocket消息的统一格式
public class Message
{
    // 消息类型（如"attack"、"attack_result"、"error"）
    public required string Type { get; set; }

    // 消息内容（JSON字符串）
    public required string Data { get; set; }
}