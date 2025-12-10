using Microsoft.AspNetCore.Mvc;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Query;

[QueryType]
public class UserQuery
{
    public async Task<User> GetUserById([FromServices] IUserRepository userRepository, int id)
    {
        return await userRepository.GetUserById(id);
    }

    public async Task<List<User>> GetAllUsers([FromServices] IUserRepository userRepository)
    {
        return  await userRepository.GetAllUsers();
    }
}
