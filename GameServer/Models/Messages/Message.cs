namespace GameServer.Models;

public class Message
{
    public string Type { get; set; } // ��Ϣ���ͣ�"login"��"attack"��
    public string Data { get; set; } // ��Ϣ���ݣ�JSON�ַ�����
}