namespace GameServer.Models.Entities;

public class User
{

    public string? Id { get; set; } // ����Ϊnull
    public required string Username { get; set; } // �����ʼ��
    public required string Password { get; set; }
}