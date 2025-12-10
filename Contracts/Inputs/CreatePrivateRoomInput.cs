namespace R2yChatSystem.Contracts.Inputs;

public class CreatePrivateRoomInput
{
    public string UserEmail { get; set; } = string.Empty;
    public required string CreatorEmail { get; set; } = string.Empty;
}