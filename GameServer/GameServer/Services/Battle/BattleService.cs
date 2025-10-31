using GameServer.Models.Enums;
using GameServer.Models.Messages;

namespace GameServer.Services.Battle;

public class BattleService : IBattleService
{
    // 计算伤害的核心逻辑
    public AttackResult CalculateDamage(AttackRequest request)
    {
        // 简单逻辑：伤害 = 攻击方攻击力 - 防守方防御力（最低1点伤害）
        var damage = Math.Max(1, request.AttackerAttack - request.DefenderDefense);

        // 阵营加成：玩家攻击敌人有20%额外伤害
        if (request.AttackerCamp == CampType.Player && request.DefenderCamp == CampType.Enemy)
        {
            damage = (int)(damage * 1.2);
        }

        // 计算剩余血量（最低0）
        var remainingHealth = Math.Max(0, request.DefenderHealth - damage);

        // 返回攻击结果
        return new AttackResult
        {
            AttackerId = request.AttackerId,
            DefenderId = request.DefenderId,
            Damage = damage,
            RemainingHealth = remainingHealth,
            IsDefenderDead = remainingHealth == 0, // 是否死亡
            Timestamp = DateTime.Now // 时间戳
        };
    }
}