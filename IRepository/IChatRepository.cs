using R2yChatSystem.Contracts.Inputs;
using R2yChatSystem.Contracts.Types;

namespace R2yChatSystem.IRepository;

public interface IChatRepository
{
    Task<List<ChatRoom>> GetAllChatRooms(CancellationToken cancellationToken = default);
    Task<ChatRoom> GetChatRoomById(Guid id, CancellationToken cancellationToken = default);
    Task<List<ChatRoom>> GetAllChatRoomByUserEmail(string userEmail, CancellationToken cancellationToken = default);
    Task<List<Message>> GetAllMessagesByChatRoomId(Guid chatRoomId, CancellationToken cancellationToken = default);
    Task<List<ChatRoomParticipant>> GetAllParticipantsByChatRoomId(Guid chatRoomId, CancellationToken cancellationToken = default);
    Task<Message> GetMessageById(Guid id, CancellationToken cancellationToken = default);
    Task<Message> SendMessage(CreateMessageInput message, CancellationToken cancellationToken = default);
    Task<Message> VoteOnPoll(VotePollInput input, CancellationToken cancellationToken = default);
    Task<ChatRoom> CreateGroupRoom(CreateGroupRoomInput chatRoom, CancellationToken cancellationToken = default);
    Task<ChatRoom> CreatePrivateRoom(CreatePrivateRoomInput input, CancellationToken cancellationToken = default);
    Task DeleteChatRoom(Guid id, CancellationToken cancellationToken = default);
}