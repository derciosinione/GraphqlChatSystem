using R2yChatSystem.Contracts.Types;
using R2yChatSystem.Model;

namespace R2yChatSystem.Mappers;

public static class ChatMapperExtensions
{
    public static List<ChatRoom> ToResponse(this  IEnumerable<ChatRoomDbModel> chatRoomDbModels)
    {
        return chatRoomDbModels.Select(r => r.ToResponse()).ToList();
    }
}