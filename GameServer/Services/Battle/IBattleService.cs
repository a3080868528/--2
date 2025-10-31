// Services/Battle/IBattleService.cs
using GameServer.Models.Entities;
using GameServer.Models.Messages; // ����AttackRequest��AttackResult�������ռ�

namespace GameServer.Services.Battle;

public interface IBattleService
{
    // �����BattleServiceһ���ķ���
    AttackResult CalculateDamage(AttackRequest request);
}