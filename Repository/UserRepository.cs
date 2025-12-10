using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Repository;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users;
    private int _nextId;

    public UserRepository()
    {
        _users = new List<User>();

        GenerateRandomUsers(20);
        _nextId = _users.Max(u => u.Id) + 1;
    }

    private void GenerateRandomUsers(int count)
    {
        var firstNames = new[] { "James", "Mary", "Robert", "Patricia", "John", "Jennifer", "Michael", "Linda", "David", "Elizabeth", "William", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin" };
        var domains = new[] { "example.com", "test.org", "demo.net", "sample.io" };

        var random = new Random();
        var startId = _users.Count + 1;

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var domain = domains[random.Next(domains.Length)];

            var name = $"{firstName} {lastName}";
            var email = $"{firstName.ToLower()}.{lastName.ToLower()}@{domain}";

            _users.Add(new(startId + i, name, email));
        }
    }

    public async Task<User> GetUserById(int id)
    {
        return (await Task.FromResult(_users.FirstOrDefault(u => u.Id == id)))!;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await Task.FromResult(_users.ToList());
    }

    public async Task<User> AddUser(User user)
    {
        var newUser = user with { Id = _nextId++ };
        _users.Add(newUser);
        return await Task.FromResult(newUser);
    }
}
