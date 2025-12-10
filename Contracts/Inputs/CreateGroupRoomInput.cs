namespace R2yChatSystem.Contracts.Inputs;

public class CreateGroupRoomInput
{
    public required string Name { get; set; } = string.Empty;
    public List<string> ParticipantEmails { get; set; } = [];
    public string CreatorEmail { get; set; } = string.Empty;
}