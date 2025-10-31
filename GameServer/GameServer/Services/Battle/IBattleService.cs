// Services/Battle/IBattleService.cs
using GameServer.Models.Messages;  // 修正引用，指向正确的AttackResult命名空间

namespace GameServer.Services.Battle;

public interface IBattleService
{
    // 计算伤害的方法
    AttackResult CalculateDamage(AttackRequest request);
}