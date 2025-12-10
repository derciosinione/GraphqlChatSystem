using Microsoft.AspNetCore.Mvc;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Query;

[QueryType]
public class UserQuery
{
    public async Task<User> GetUserByEmail([FromServices] IUserRepository userRepository, string email)
    {
        return await userRepository.GetUserByEmail(email);
    }

    public async Task<List<User>> GetAllUsers([FromServices] IUserRepository userRepository)
    {
        return await userRepository.GetAllUsers();
    }
}
