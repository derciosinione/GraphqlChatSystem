using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Query;

[QueryType]
public class ChatQuery
{
    public async Task<List<ChatRoom>> GetChatRooms(IChatRepository chatRepository)
    {
        return await chatRepository.GetAllChatRooms();
    }

    public async Task<ChatRoom> GetChatRoom(IChatRepository chatRepository, Guid id)
    {
        return await chatRepository.GetChatRoomById(id);
    }

    public async Task<List<ChatRoom>> GetAllChatRoomByUserEmail(IChatRepository chatRepository, string userEmail)
    {
        return await chatRepository.GetAllChatRoomByUserEmail(userEmail);
    }
    
    public async Task<List<Message>> GetAllChatRoomByUserEmail(IChatRepository chatRepository, Guid roomId)
    {
        return await chatRepository.GetAllMessagesByChatRoomId(roomId);
    }
}
