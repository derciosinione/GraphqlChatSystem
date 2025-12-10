namespace R2yChatSystem.Contracts.Types;

public class PollOption
{
    public int Id { get; set; }
    public int PollId { get; set; }
    public string Text { get; set; }
    public List<PollVote> Votes { get; set; } = new();
}
