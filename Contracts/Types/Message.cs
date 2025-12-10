namespace R2yChatSystem.Contracts.Types;

public class Message
{
    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public int UserId { get; set; }
    public int? ReplyToMessageId { get; set; }
    public int? PollId { get; set; }
    public string? Content { get; set; }
    public string? FileUrl { get; set; }
    public DateTime SentAt { get; set; } = DateTime.Now;
    public List<User?> ReadBy { get; set; } = new();
}

