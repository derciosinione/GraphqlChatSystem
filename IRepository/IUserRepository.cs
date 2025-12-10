using R2yChatSystem.Contracts.Types;

namespace R2yChatSystem.IRepository;

public interface IUserRepository
{
    Task<User> GetUserById(int id);
    Task<List<User>> GetAllUsers();
    Task<User> AddUser(User user);
}