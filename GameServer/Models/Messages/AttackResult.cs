namespace GameServer.Models.Entities;

// ������������ظ�ǰ�ˣ�
public class AttackResult
{

    public string? AttackId { get; set; }
    public string? TargetId { get; set; }
    public string? FinalDamage { get; set; }
    public string? RemainingHp { get; set; }
}