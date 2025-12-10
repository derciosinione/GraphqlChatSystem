using R2yChatSystem.Contracts.Enum;

namespace R2yChatSystem.Contracts.Types;

public record Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public MessageType Type { get; set; } = MessageType.Text;
    public Guid ChatRoomId { get; set; }
    public User Sender { get; set; } = null!;
    public string? Content { get; set; }
    public string? FileUrl { get; set; }
    public Message? ReplyToMessage { get; set; }
    public Poll? Poll { get; set; }
    public List<MessageRead?> ReadBy { get; set; } = [];
    public DateTime SentAt { get; set; } = DateTime.Now;
}

public record MessageRead
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserEmail { get; set; } = string.Empty;
    public DateTime ReadAt { get; set; } = DateTime.Now;
}

public record Poll
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Question { get; set; }
    public bool IsMultipleChoice { get; set; } = false;
    public List<PollOption> Options { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ExpiresAt { get; set; }
}

public record PollOption
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Text { get; set; }
    public List<PollVote> Votes { get; set; } = [];
}

public record PollVote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserEmail { get; set; } = string.Empty;
    public DateTime VotedAt { get; set; } = DateTime.Now;
}
