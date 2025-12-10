using R2yChatSystem.Contracts.Inputs;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Repository;

public class ChatRepository : IChatRepository
{
    private List<ChatRoom> ChatRooms = [];
    private int _nextId;
    private readonly IUserRepository _userRepository;
    private bool _isInitialized;

    public ChatRepository(IUserRepository userRepository)
    {
        ChatRooms = [];
        _userRepository = userRepository;
        _isInitialized = false;
    }

    private async Task EnsureInitialized()
    {
        if (_isInitialized) return;

        await GenerateMockData();
        _nextId = ChatRooms.Count != 0 ? ChatRooms.Max(c => c.Id) + 1 : 1;
        _isInitialized = true;
    }

    private async Task GenerateMockData()
    {
        // var random = new Random();
        //
        // var roomNames = new[]
        // {
        //     "General", "Random", "Dev Team", "Design", "Marketing", "Sales", "Support", "HR", "Operations", "Strategy"
        // };

        // var allUsers = await _userRepository.GetAllUsers();
        // if (allUsers.Count == 0) return; // Should not happen given UserRepository implementation
        //
        // // Generate 25 mock rooms
        // for (var i = 1; i <= 25; i++)
        // {
        //     var isGroup = random.NextDouble() > 0.3; // 70% chance of group room
        //     var roomType = isGroup ? RoomType.Group : RoomType.Private;
        //     var participants = GenerateRandomParticipants(random, allUsers, i, roomType);
        //
        //     var name = isGroup
        //         ? roomNames[random.Next(roomNames.Length)] + " " + random.Next(1, 100)
        //         : null; // Private rooms have null name
        //
        //     var room = new ChatRoom
        //     {
        //         Id = i,
        //         Type = roomType,
        //         Name = name,
        //         CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
        //         Participants = participants
        //     };
        //
        //     _chatRooms.Add(room);
        // }
    }

    private List<ChatRoomParticipant> GenerateRandomParticipants(Random random, List<User> allUsers, int roomId, RoomType type)
    {
        // Private: exactly 2. Group: 2 to 6.
        var count = type == RoomType.Private ? 2 : random.Next(2, 6);

        // Pick random unique users from allUsers
        var shuffledUsers = allUsers.OrderBy(x => random.Next()).Take(count).ToList();

        return shuffledUsers.Select((t, i) => new ChatRoomParticipant
            {
                Id = random.Next(1000, 9999),
                ChatRoomId = roomId,
                UserEmail = t.Email,
                Role = (i == 0 && type == RoomType.Group) ? Role.Admin : Role.Member, // Only group has clear Admin usually, but let's keep first as Admin or Member
                JoinedAt = DateTime.Now.AddDays(-random.Next(1, 100)),
                User = t
            })
            .ToList();
    }

    public async Task<List<ChatRoom>> GetAllChatRooms()
    {
        await EnsureInitialized();
        return await Task.FromResult(ChatRooms);
    }

    public async Task<ChatRoom> GetChatRoomById(int id)
    {
        await EnsureInitialized();
        var room = ChatRooms.FirstOrDefault(c => c.Id == id);
        if (room == null) throw new Exception("Chat room not found");
        return await Task.FromResult(room);
    }

    public async Task<ChatRoom> CreateGroupRoom(CreateGroupRoomInput chatRoom)
    {
        await EnsureInitialized();

        var participants = new List<ChatRoomParticipant>();
        var roomId = _nextId++;

        // Add Creator as Admin
        var creator = await _userRepository.GetUserByEmail(chatRoom.CreatorEmail);
        if (creator != null)
        {
            participants.Add(new ChatRoomParticipant
            {
                Id = new Random().Next(10000, 99999),
                ChatRoomId = roomId,
                UserEmail = creator.Email,
                Role = Role.Admin,
                JoinedAt = DateTime.Now,
                User = creator
            });
        }

        foreach (var email in chatRoom.UserEmails)
        {
            // Skip if creator is also in the list (avoid duplicates)
            if (email == chatRoom.CreatorEmail) continue;

            var user = await _userRepository.GetUserByEmail(email);
            if (user != null)
            {
                participants.Add(new ChatRoomParticipant
                {
                    Id = new Random().Next(10000, 99999),
                    ChatRoomId = roomId,
                    UserEmail = user.Email,
                    Role = Role.Member,
                    JoinedAt = DateTime.Now,
                    User = user
                });
            }
        }

        var newRoom = new ChatRoom
        {
            Id = roomId,
            Name = chatRoom.Name,
            Type = RoomType.Group,
            CreatedAt = DateTime.Now,
            Participants = participants
        };

        ChatRooms.Add(newRoom);
        return await Task.FromResult(newRoom);
    }

    public async Task<ChatRoom> CreatePrivateRoom(CreatePrivateRoomInput input)
    {
        await EnsureInitialized();

        var participants = new List<ChatRoomParticipant>();
        var roomId = _nextId++;

        // Add Creator
        var creator = await _userRepository.GetUserByEmail(input.CreatorEmail);
        if (creator != null)
        {
            participants.Add(new ChatRoomParticipant
            {
                Id = new Random().Next(10000, 99999),
                ChatRoomId = roomId,
                UserEmail = creator.Email,
                Role = Role.Member, // Using Member for private chat equality
                JoinedAt = DateTime.Now,
                User = creator
            });
        }

        // Add Target User
        if (input.UserEmail != input.CreatorEmail)
        {
            var user = await _userRepository.GetUserByEmail(input.UserEmail);
            if (user != null)
            {
                participants.Add(new ChatRoomParticipant
                {
                    Id = new Random().Next(10000, 99999),
                    ChatRoomId = roomId,
                    UserEmail = user.Email,
                    Role = Role.Member,
                    JoinedAt = DateTime.Now,
                    User = user
                });
            }
        }

        var newRoom = new ChatRoom
        {
            Id = roomId,
            Name = null, // Private room has no name
            Type = RoomType.Private,
            CreatedAt = DateTime.Now,
            Participants = participants
        };

        ChatRooms.Add(newRoom);
        return await Task.FromResult(newRoom);
    }

    public async Task DeleteChatRoom(int id)
    {
        await EnsureInitialized();
        var room = ChatRooms.FirstOrDefault(c => c.Id == id);
        if (room != null)
        {
            ChatRooms.Remove(room);
        }

        await Task.CompletedTask;
    }
}