using R2yChatSystem.Contracts.Enum;

namespace R2yChatSystem.Contracts.Types;

public class ChatRoomParticipant
{
    public Guid Id  { get; set; } = Guid.NewGuid();
    public Guid ChatRoomId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.Member;
    public DateTime JoinedAt { get; set; } = DateTime.Now;
    public User User { get; set; } = null!;
}

