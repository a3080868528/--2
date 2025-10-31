namespace GameServer.Models.Messages;

public class AttackResult
{
    public bool Success { get; set; }
    public int Damage { get; set; }
    public int RemainingHealth { get; set; }
    public DateTime Timestamp { get; set; }
}