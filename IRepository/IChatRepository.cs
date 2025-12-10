using R2yChatSystem.Contracts.Inputs;
using R2yChatSystem.Contracts.Types;

namespace R2yChatSystem.IRepository;

public interface IChatRepository
{
    Task<List<ChatRoom>> GetAllChatRooms();
    Task<ChatRoom> GetChatRoomById(int id);
    Task<ChatRoom> CreateGroupRoom(CreateGroupRoomInput chatRoom);
    Task<ChatRoom> CreatePrivateRoom(CreatePrivateRoomInput input);
    Task DeleteChatRoom(int id);
}