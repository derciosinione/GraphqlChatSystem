using R2yChatSystem.Contracts.Enum;

namespace R2yChatSystem.Contracts.Types;

public class ChatRoom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public RoomType Type { get; set; } = RoomType.Private;
    public string? Name { get; set; }
    public List<ChatRoomParticipant> Participants { get; set; } = [];
    public List<Message> Messages { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ChatRoom ShallowCopy()
    {
        var roomView = new ChatRoom
        {
            Id = Id,
            Type = Type,
            CreatedAt = CreatedAt,
            Participants = Participants,
            Name = Name,
            Messages = Messages
        };
        
        return roomView;
    }
}


public record ChatRoomParticipant
{
    public User User { get; set; } = null!;
    public Role Role { get; set; } = Role.Member;
    public DateTime JoinedAt { get; set; } = DateTime.Now;

    public static ChatRoomParticipant Add(User destinationUser = null!, Role role = Role.Member)
    {
        ArgumentNullException.ThrowIfNull(destinationUser);
        
        return new ChatRoomParticipant
        {
            Role = role,
            JoinedAt = DateTime.Now,
            User = destinationUser
        };
    }
}

