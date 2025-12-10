namespace R2yChatSystem.Contracts.Inputs;

public class CreatePrivateRoomInput
{
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
}