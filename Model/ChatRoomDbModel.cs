using System.Text.Json;
using R2yChatSystem.Contracts.Enum;
using R2yChatSystem.Contracts.Types;

namespace R2yChatSystem.Model;

public class ChatRoomDbModel
{
    public Guid Id { get; set; }
    public RoomType Type { get; set; }
    public string? Name { get; set; }
    public string? Participants { get; set; }
    public string? Messages { get; set; }
    public DateTime CreatedAt { get; set; }

    public ChatRoom ToResponse()
    {
        return new ChatRoom
        {
            Id = Id,
            Type = Type,
            Name = Name,
            Participants = Participants == null ? [] : JsonSerializer.Deserialize<List<ChatRoomParticipant>>(Participants) ?? [],
            Messages = Messages == null ? [] : JsonSerializer.Deserialize<List<Message>>(Messages) ?? [],
            CreatedAt = CreatedAt
        };
    }
}
