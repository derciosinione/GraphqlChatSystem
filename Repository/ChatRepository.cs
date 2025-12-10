using R2yChatSystem.Contracts.Inputs;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Repository;

public class ChatRepository : IChatRepository
{
    private readonly List<ChatRoom> _chatRooms;
    private int _nextId;

    public ChatRepository()
    {
        _chatRooms = [];
        GenerateMockData();
        _nextId = _chatRooms.Count != 0 ? _chatRooms.Max(c => c.Id) + 1 : 1;
    }

    private void GenerateMockData()
    {
        var random = new Random();
        
        var roomNames = new[]
        {
            "General", "Random", "Dev Team", "Design", "Marketing", "Sales", "Support", "HR", "Operations", "Strategy"
        };

        // Generate 25 mock rooms
        for (var i = 1; i <= 25; i++)
        {
            var isGroup = random.NextDouble() > 0.3; // 70% chance of group room
            var roomType = isGroup ? RoomType.Group : RoomType.Private;
            var name = isGroup ? roomNames[random.Next(roomNames.Length)] + " " + random.Next(1, 100) : string.Empty;

            var room = new ChatRoom
            {
                Id = i,
                Type = roomType,
                Name = name,
                CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
                Users = GenerateRandomUsers(random)
            };

            _chatRooms.Add(room);
        }
    }

    private static List<User> GenerateRandomUsers(Random random)
    {
        var users = new List<User>();
        var count = random.Next(2, 6);
        for (var i = 0; i < count; i++)
        {
            users.Add(new User(random.Next(1, 100), $"User{random.Next(100, 999)}",
                $"user{random.Next(100, 999)}@example.com"));
        }

        return users;
    }

    public async Task<List<ChatRoom>> GetAllChatRooms()
    {
        return await Task.FromResult(_chatRooms);
    }

    public async Task<ChatRoom> GetChatRoomById(int id)
    {
        var room = _chatRooms.FirstOrDefault(c => c.Id == id);
        if (room == null) throw new Exception("Chat room not found");
        return await Task.FromResult(room);
    }

    public async Task<ChatRoom> CreateGroupRoom(CreateGroupRoomInput chatRoom)
    {
        var newRoom = new ChatRoom
        {
            Id = _nextId++,
            Name = chatRoom.Name,
            Type = RoomType.Group,
            CreatedAt = DateTime.Now,
            // In a real app we'd fetch actual users by ID, but for mock we'll just add placeholder users if IDs are provided
            Users = chatRoom.UserIds.Select(id => new User(id, $"User{id}", $"user{id}@test.com")).ToList()
        };

        _chatRooms.Add(newRoom);
        return await Task.FromResult(newRoom);
    }

    public async Task<ChatRoom> CreatePrivateRoom(CreatePrivateRoomInput input)
    {
        // For private room, usually name isn't required or is auto-generated based on participants
        var newRoom = new ChatRoom
        {
            Id = _nextId++,
            Name = input.Name,
            Type = RoomType.Private,
            CreatedAt = DateTime.Now,
            Users = [new User(input.UserId, $"User{input.UserId}", $"user{input.UserId}@test.com")]
        };

        _chatRooms.Add(newRoom);
        return await Task.FromResult(newRoom);
    }

    public async Task DeleteChatRoom(int id)
    {
        var room = _chatRooms.FirstOrDefault(c => c.Id == id);
        if (room != null)
        {
            _chatRooms.Remove(room);
        }

        await Task.CompletedTask;
    }
}