using R2yChatSystem.Contracts.Enum;

namespace R2yChatSystem.Contracts.Types;

public class ChatRoom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public RoomType Type { get; set; } = RoomType.Private;
    public string? Name { get; set; }
    public List<ChatRoomParticipant> Participants { get; set; } = [];
    public List<Message> Messages { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}


public record ChatRoomParticipant
{
    public string UserEmail { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public Role Role { get; set; } = Role.Member;
    public DateTime JoinedAt { get; set; } = DateTime.Now;
}

