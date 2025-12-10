using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Repository;

public class UserRepository : IUserRepository
{
    private List<User> _users;

    public UserRepository()
    {
        _users = [];
        GenerateRandomUsers();
    }

    private void GenerateRandomUsers()
    {
        _users =
        [
            new User("Ana Silva", "ana.silva@example.com"),
            new User("Bruno Costa", "bruno.costa@example.com"),
            new User("Carla Mendes", "carla.mendes@example.com"),
            new User("Daniel Rocha", "daniel.rocha@example.com"),
            new User("Eduarda Pinto", "eduarda.pinto@example.com"),
            new User("Fábio Matos", "fabio.matos@example.com"),
            new User("Gabriela Sousa", "gabriela.sousa@example.com"),
            new User("Henrique Dias", "henrique.dias@example.com"),
            new User("Inês Barros", "ines.barros@example.com"),
            new User("João Ribeiro", "joao.ribeiro@example.com"),
            new User("Kátia Ramos", "katia.ramos@example.com"),
            new User("Leonardo Reis", "leonardo.reis@example.com"),
            new("Mariana Lopes", "mariana.lopes@example.com"),
            new("Nuno Carvalho", "nuno.carvalho@example.com"),
            new("Óscar Figueiredo", "oscar.figueiredo@example.com"),
            new("Patrícia Amaral", "patricia.amaral@example.com"),
            new("Quésia Martins", "quesia.martins@example.com"),
            new("Rafael Gomes", "rafael.gomes@example.com"),
            new("Sara Cunha", "sara.cunha@example.com"),
            new("Tiago Ferraz", "tiago.ferraz@example.com"),
            new("Ursula Ribeiro", "ursula.ribeiro@example.com"),
            new("Vasco Moura", "vasco.moura@example.com"),
            new("William Duarte", "william.duarte@example.com"),
            new("Xavier Pires", "xavier.pires@example.com"),
            new("Yara Fontes", "yara.fontes@example.com"),
            new("Zé Luís", "ze.luis@example.com"),
            new("Beatriz Serra", "beatriz.serra@example.com"),
            new("Cristiano Vale", "cristiano.vale@example.com"),
            new("Diana Rocha", "diana.rocha@example.com")
        ];
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return (await Task.FromResult(_users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase))))!;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await Task.FromResult(_users.ToList());
    }

    public async Task<User> AddUser(User user)
    {
        _users.Add(user);
        return await Task.FromResult(user);
    }
}