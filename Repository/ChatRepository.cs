using R2yChatSystem.Contracts.Inputs;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Repository;

public class ChatRepository : IChatRepository
{
    public async Task<List<ChatRoom>> GetAllChatRooms()
    {
        throw new NotImplementedException();
    }

    public async Task<ChatRoom> GetChatRoomById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ChatRoom> CreateGroupRoom(CreateGroupRoomInput chatRoom)
    {
        throw new NotImplementedException();
    }

    public async Task<ChatRoom> CreatePrivateRoom(CreatePrivateRoomInput input)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteChatRoom(int id)
    {
        throw new NotImplementedException();
    }
}