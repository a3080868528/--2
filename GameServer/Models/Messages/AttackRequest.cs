using GameServer.Models.Enums;

namespace GameServer.Models.Messages;

public class AttackRequest
{
    public string AttackerId { get; set; }
    public string DefenderId { get; set; }
    public int AttackValue { get; set; }
    public int DefenderHealth { get; set; }
    public CampType AttackerCamp { get; set; }
    public CampType DefenderCamp { get; set; }
}