namespace GameServer.Models.Messages;

// ����WebSocket��Ϣ��ͳһ��ʽ
public class Message
{
    // ��Ϣ���ͣ���"attack"��"attack_result"��"error"��
    public required string Type { get; set; }

    // ��Ϣ���ݣ�JSON�ַ�����
    public required string Data { get; set; }
}