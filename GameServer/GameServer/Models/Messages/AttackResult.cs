namespace GameServer.Models.Messages;

// ������������������ظ��ͻ��ˣ�
public class AttackResult
{
    // ������ID
    public string AttackerId { get; set; } = string.Empty;

    // ������ID
    public string DefenderId { get; set; } = string.Empty;

    // ��ɵ��˺�
    public int Damage { get; set; }

    // ������ʣ��Ѫ��
    public int RemainingHealth { get; set; }

    // �������Ƿ�����
    public bool IsDefenderDead { get; set; }

    // ����ʱ��
    public DateTime Timestamp { get; set; }
}