using GameServer.Models.Enums;

namespace GameServer.Models.Messages;

// �����������ݣ��ͻ��˷��͸���������
public class AttackRequest
{
    // ������ID��������д��
    public required string AttackerId { get; set; }

    // ������ID��������д��
    public required string DefenderId { get; set; }

    // �����߹�����
    public int AttackerAttack { get; set; }

    // �����߷�����
    public int DefenderDefense { get; set; }

    // �����ߵ�ǰѪ��
    public int DefenderHealth { get; set; }

    // ��������Ӫ
    public CampType AttackerCamp { get; set; }

    // ��������Ӫ
    public CampType DefenderCamp { get; set; }
}