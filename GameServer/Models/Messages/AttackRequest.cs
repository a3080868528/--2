
namespace GameServer.Models.Messages;
// �����������ݽṹ��ǰ�˷��ͣ�
public class AttackRequest
{
    public required string AttackId { get; set; }   // ������ID
    public required string TargetId { get; set; }   // Ŀ��ID
    public required string AttackValue { get; set; }   // ����ֵ
}