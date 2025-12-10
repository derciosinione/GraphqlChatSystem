
namespace R2yChatSystem.Contracts.Inputs;

public class CreatePollInput
{
    public required string Question { get; set; }
    public List<string> Options { get; set; } = [];
    public bool IsMultipleChoice { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
