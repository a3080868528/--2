using GameServer.Models.Enums;
using GameServer.Models.Messages;

namespace GameServer.Services.Battle;

public class BattleService : IBattleService
{
    // �����˺��ĺ����߼�
    public AttackResult CalculateDamage(AttackRequest request)
    {
        // ���߼����˺� = ������������ - ���ط������������1���˺���
        var damage = Math.Max(1, request.AttackerAttack - request.DefenderDefense);

        // ��Ӫ�ӳɣ���ҹ���������20%�����˺�
        if (request.AttackerCamp == CampType.Player && request.DefenderCamp == CampType.Enemy)
        {
            damage = (int)(damage * 1.2);
        }

        // ����ʣ��Ѫ�������0��
        var remainingHealth = Math.Max(0, request.DefenderHealth - damage);

        // ���ع������
        return new AttackResult
        {
            AttackerId = request.AttackerId,
            DefenderId = request.DefenderId,
            Damage = damage,
            RemainingHealth = remainingHealth,
            IsDefenderDead = remainingHealth == 0, // �Ƿ�����
            Timestamp = DateTime.Now // ʱ���
        };
    }
}