namespace R2yChatSystem.Contracts.Types;

public class MessageRead
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public DateTime ReadAt { get; set; } = DateTime.Now;
}