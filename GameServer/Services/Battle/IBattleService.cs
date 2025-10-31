using GameServer.Models.Entities;
using GameServer.Models.Messages;

namespace GameServer.Services.Battle;

public interface IBattleService
{
    AttackResult CalculateDamage(AttackRequest request);
}