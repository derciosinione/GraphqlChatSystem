using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Query;

[QueryType]
public class UserQuery
{
    public async Task<User> GetUserByEmail(IUserRepository userRepository, string email)
    {
        return await userRepository.GetUserByEmail(email);
    }

    public async Task<List<User>> GetAllUsers(IUserRepository userRepository)
    {
        return await userRepository.GetAllUsers();
    }
}
