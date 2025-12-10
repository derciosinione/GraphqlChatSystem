namespace R2yChatSystem.Contracts.Types;

public class ChatRoom
{
    public int Id { get; set; }
    public RoomType Type { get; set; } = RoomType.Private;
    public string Name { get; set; } = string.Empty;
    public List<User> Users { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum RoomType { Private, Group }
