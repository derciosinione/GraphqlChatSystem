using R2yChatSystem.Contracts.Enum;

namespace R2yChatSystem.Contracts.Types;

public class ChatRoom
{
    public Guid Id  { get; set; } = Guid.NewGuid();
    public RoomType Type { get; set; } = RoomType.Private;
    public string? Name { get; set; }
    public List<ChatRoomParticipant> Participants { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

