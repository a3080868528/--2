// Services/Battle/IBattleService.cs
using GameServer.Models.Messages;  // �������ã�ָ����ȷ��AttackResult�����ռ�

namespace GameServer.Services.Battle;

public interface IBattleService
{
    // �����˺��ķ���
    AttackResult CalculateDamage(AttackRequest request);
}