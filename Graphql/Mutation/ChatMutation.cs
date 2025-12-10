using R2yChatSystem.Contracts.Inputs;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Mutation;

[MutationType]
public class ChatMutation
{
    public async Task<ChatRoom> CreateGroupRoom(IChatRepository chatRepository, CreateGroupRoomInput input)
    {
        return await chatRepository.CreateGroupRoom(input);
    }

    public async Task<ChatRoom> CreatePrivateRoom(IChatRepository chatRepository, CreatePrivateRoomInput input)
    {
        return await chatRepository.CreatePrivateRoom(input);
    }

    public async Task<bool> DeleteChatRoom(IChatRepository chatRepository, Guid id)
    {
        await chatRepository.DeleteChatRoom(id);
        return true;
    }
    
    public async Task<Message> SendMessage(IChatRepository chatRepository, CreateMessageInput input)
    {
        return await chatRepository.SendMessage(input);
    }

    public async Task<Message> VoteOnPoll(IChatRepository chatRepository, VotePollInput input)
    {
        return await chatRepository.VoteOnPoll(input);
    }
}
