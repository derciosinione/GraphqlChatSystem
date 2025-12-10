namespace R2yChatSystem.Contracts.Inputs;

public class CreateGroupRoomInput
{
    public string Name { get; set; } = string.Empty;
    public List<string> UserEmails { get; set; } = [];
    public string CreatorEmail { get; set; } = string.Empty;
}