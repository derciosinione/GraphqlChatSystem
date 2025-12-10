namespace R2yChatSystem.Contracts.Types;

public class PollVote
{
    public int Id { get; set; }
    public int PollOptionId { get; set; }
    public int UserId { get; set; }
    public DateTime VotedAt { get; set; } = DateTime.Now;
}
