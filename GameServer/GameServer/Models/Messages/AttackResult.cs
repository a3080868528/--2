namespace GameServer.Models.Messages;

// 攻击结果（服务器返回给客户端）
public class AttackResult
{
    // 攻击者ID
    public string AttackerId { get; set; } = string.Empty;

    // 防守者ID
    public string DefenderId { get; set; } = string.Empty;

    // 造成的伤害
    public int Damage { get; set; }

    // 防守者剩余血量
    public int RemainingHealth { get; set; }

    // 防守者是否死亡
    public bool IsDefenderDead { get; set; }

    // 攻击时间
    public DateTime Timestamp { get; set; }
}