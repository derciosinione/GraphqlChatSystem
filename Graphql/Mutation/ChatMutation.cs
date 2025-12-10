using R2yChatSystem.Contracts.Inputs;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Mutation;

[MutationType]
public class ChatMutation
{
    public async Task<ChatRoom> CreateGroupRoom([Service] IChatRepository chatRepository, CreateGroupRoomInput input)
    {
        return await chatRepository.CreateGroupRoom(input);
    }

    public async Task<ChatRoom> CreatePrivateRoom([Service] IChatRepository chatRepository, CreatePrivateRoomInput input)
    {
        return await chatRepository.CreatePrivateRoom(input);
    }

    public async Task<bool> DeleteChatRoom([Service] IChatRepository chatRepository, Guid id)
    {
        await chatRepository.DeleteChatRoom(id);
        return true;
    }
}
