using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Query;

[QueryType]
public class ChatQuery
{
    public async Task<List<ChatRoom>> GetChatRooms([Service] IChatRepository chatRepository)
    {
        return await chatRepository.GetAllChatRooms();
    }

    public async Task<ChatRoom> GetChatRoom([Service] IChatRepository chatRepository, Guid id)
    {
        return await chatRepository.GetChatRoomById(id);
    }

    public async Task<List<ChatRoom>> GetAllChatRoomByUserEmail([Service] IChatRepository chatRepository, string userEmail)
    {
        return await chatRepository.GetAllChatRoomByUserEmail(userEmail);
    }
}
