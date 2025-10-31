using GameServer.Models.Entities;
using GameServer.Models.Enums;
using GameServer.Models.Messages;

namespace GameServer.Services.Battle;

public class BattleService : IBattleService
{
    public AttackResult CalculateDamage(AttackRequest request)
    {
        // 简单的伤害计算逻辑
        var random = new Random();
        var damage = random.Next(10, 50) + request.AttackValue;

        // 阵营加成逻辑
        if (request.AttackerCamp == CampType.Player && request.DefenderCamp == CampType.Enemy)
        {
            damage = (int)(damage * 1.2f); // 玩家对敌人有20%伤害加成
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