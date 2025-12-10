using R2yChatSystem.Contracts.Types;

namespace R2yChatSystem.Graphql.Mutation;

[MutationType]
public class ChatMutation
{
    public  User AddUser(User user)
        => user;
}
