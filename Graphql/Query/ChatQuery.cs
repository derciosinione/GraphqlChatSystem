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

    public async Task<ChatRoom> GetChatRoom([Service] IChatRepository chatRepository, int id)
    {
        return await chatRepository.GetChatRoomById(id);
    }
}
