namespace R2yChatSystem.Contracts.Inputs;

public class CreateGroupRoomInput
{
    public string Name { get; set; } = string.Empty;
    public List<int> UserIds { get; set; } = [];
}