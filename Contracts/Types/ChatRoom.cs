namespace R2yChatSystem.Contracts.Types;

public class ChatRoom
{
    public int Id { get; set; }
    public RoomType Type { get; set; } = RoomType.Private;
    public string? Name { get; set; }
    public List<ChatRoomParticipant> Participants { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum RoomType { Private, Group }
