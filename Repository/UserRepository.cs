using Dapper;
using Microsoft.Data.SqlClient;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Repository;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        InitializeDatabase().Wait();
    }

    private async Task InitializeDatabase()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var tableExists = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users'");

        if (tableExists == 0)
        {
            await connection.ExecuteAsync("""
                                              CREATE TABLE Users (
                                                  Email NVARCHAR(255) PRIMARY KEY,
                                                  Name NVARCHAR(255) NOT NULL
                                              )
                                          """);
        }
    }


    public async Task<User?> GetUserByEmail(string email)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Email = @Email",
            new { Email = email });
    }

    public async Task<List<User>> GetAllUsers()
    {
        await using var connection = new SqlConnection(_connectionString);
        var users = await connection.QueryAsync<User>("SELECT * FROM Users");
        return users.ToList();
    }

    public async Task<User> AddUser(User user)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("INSERT INTO Users (Name, Email) VALUES (@Name, @Email)", user);
        return user;
    }
}