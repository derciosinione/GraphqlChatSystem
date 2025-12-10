
namespace R2yChatSystem.Contracts.Inputs;

public class VotePollInput
{
    public Guid MessageId { get; set; }
    public Guid OptionId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
}
