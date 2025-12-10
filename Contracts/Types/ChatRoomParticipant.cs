namespace R2yChatSystem.Contracts.Types;

public class ChatRoomParticipant
{
    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.Member;
    public DateTime JoinedAt { get; set; } = DateTime.Now;
    public User User { get; set; } = null!;
}

public enum Role { Admin, Member }
