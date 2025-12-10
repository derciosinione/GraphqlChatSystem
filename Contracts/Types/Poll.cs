namespace R2yChatSystem.Contracts.Types;

public class Poll
{
    public int Id { get; set; }
    public string Question { get; set; }
    public bool IsMultipleChoice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ExpiresAt { get; set; }
    public List<PollOption> Options { get; set; } = new();
}
