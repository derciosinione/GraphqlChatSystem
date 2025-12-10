namespace R2yChatSystem.Contracts.Types;

public class ChatRoomParticipant
{
    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public int UserId { get; set; }
    public Role Role { get; set; }
    public DateTime JoinedAt { get; set; }
}

public enum Role { Admin, Member }
