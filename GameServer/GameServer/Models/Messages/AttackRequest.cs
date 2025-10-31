using GameServer.Models.Enums;

namespace GameServer.Models.Messages;

// 攻击请求数据（客户端发送给服务器）
public class AttackRequest
{
    // 攻击者ID（必须填写）
    public required string AttackerId { get; set; }

    // 防守者ID（必须填写）
    public required string DefenderId { get; set; }

    // 攻击者攻击力
    public int AttackerAttack { get; set; }

    // 防守者防御力
    public int DefenderDefense { get; set; }

    // 防守者当前血量
    public int DefenderHealth { get; set; }

    // 攻击者阵营
    public CampType AttackerCamp { get; set; }

    // 防守者阵营
    public CampType DefenderCamp { get; set; }
}