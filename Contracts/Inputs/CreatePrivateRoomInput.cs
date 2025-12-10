namespace R2yChatSystem.Contracts.Inputs;

public class CreatePrivateRoomInput
{
    public string UserEmail { get; set; } = string.Empty;
    public string CreatorEmail { get; set; } = string.Empty;
}