using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Query;

[QueryType]
public class UserQuery
{
    public async Task<User> GetUserByEmail(IUserRepository userRepository, string email)
    {
        var user = await userRepository.GetUserByEmail(email) 
                   ?? throw new GraphQLException("User not found");

        return user;
    }

    public async Task<List<User>> GetAllUsers(IUserRepository userRepository)
    {
        return await userRepository.GetAllUsers();
    }
}
