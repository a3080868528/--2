using GameServer.Models.Entities;
using GameServer.Models.Enums;
using GameServer.Models.Messages;

namespace GameServer.Services.Battle;

public class BattleService : IBattleService
{
    public AttackResult CalculateDamage(AttackRequest request)
    {
        // �򵥵��˺������߼�
        var random = new Random();
        var damage = random.Next(10, 50) + request.AttackValue;

        // ��Ӫ�ӳ��߼�
        if (request.AttackerCamp == CampType.Player && request.DefenderCamp == CampType.Enemy)
        {
            damage = (int)(damage * 1.2f); // ��ҶԵ�����20%�˺��ӳ�
        }

        return new AttackResult
        {
            Success = true,
            Damage = damage,
            RemainingHealth = Math.Max(0, request.DefenderHealth - damage),
            Timestamp = DateTime.UtcNow
        };
    }
}