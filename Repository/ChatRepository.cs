using R2yChatSystem.Contracts.Enum;
using R2yChatSystem.Contracts.Inputs;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Repository;

public class ChatRepository : IChatRepository
{
    private List<ChatRoom> ChatRooms = [];
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
        _isInitialized = true;
    }

    private async Task GenerateMockData()
    {
        ChatRooms = [];

        var privateRoomId = Guid.NewGuid();
        var groupRoomId = Guid.NewGuid();

        ChatRooms.AddRange([
            new ChatRoom
            {
                Id = privateRoomId,
                Type = RoomType.Private,
                Participants = [
                    new ChatRoomParticipant
                    {
                        ChatRoomId = privateRoomId,
                        UserEmail = "ana.silva@example.com",
                        Role = Role.Admin,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("ana.silva@example.com")
                    },
                    new ChatRoomParticipant
                    {
                        ChatRoomId = privateRoomId,
                        UserEmail = "bruno.costa@example.com",
                        Role = Role.Member,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("bruno.costa@example.com")
                    }
                ],
                CreatedAt = DateTime.Now,
            },
            new ChatRoom
            {
                Id = groupRoomId,
                Type = RoomType.Group,
                Name = "Group Room",
                Participants = [
                    new ChatRoomParticipant
                    {
                        ChatRoomId = groupRoomId,
                        UserEmail = "ana.silva@example.com",
                        Role = Role.Admin,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("ana.silva@example.com")
                    },
                    new ChatRoomParticipant
                    {
                        ChatRoomId = groupRoomId,
                        UserEmail = "bruno.costa@example.com",
                        Role = Role.Member,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("bruno.costa@example.com")
                    },
                    new ChatRoomParticipant
                    {
                        ChatRoomId = groupRoomId,
                        UserEmail = "ze.luis@example.com",
                        Role = Role.Member,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("ze.luis@example.com")
                    },
                    new ChatRoomParticipant
                    {
                        ChatRoomId = groupRoomId,
                        UserEmail = "vasco.moura@example.com",
                        Role = Role.Member,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("vasco.moura@example.com")
                    }
                ],
                CreatedAt = DateTime.Now,
            }
        ]);
    }


    public async Task<List<ChatRoom>> GetAllChatRooms()
    {
        await EnsureInitialized();
        return await Task.FromResult(ChatRooms);
    }

    public async Task<ChatRoom> GetChatRoomById(Guid id)
    {
        await EnsureInitialized();
        var room = ChatRooms.FirstOrDefault(c => c.Id == id);
        if (room == null) throw new Exception("Chat room not found");
        return await Task.FromResult(room);
    }

    public async Task<List<ChatRoom>> GetAllChatRoomByUserEmail(string userEmail)
    {
        await EnsureInitialized();
        var rooms = ChatRooms.Where(c =>
            c.Participants.Any(p => p.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase))).ToList();

        var result = new List<ChatRoom>();

        foreach (var room in rooms)
        {
            // Create a shallow copy to safely modify the Name without affecting the singleton storage
            var roomView = new ChatRoom
            {
                Id = room.Id,
                Type = room.Type,
                CreatedAt = room.CreatedAt,
                Participants = room.Participants,
                Name = room.Name
            };

            if (room.Type == RoomType.Private)
            {
                var otherParticipant = room.Participants.FirstOrDefault(p =>
                    !p.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
                if (otherParticipant?.User != null)
                {
                    roomView.Name = otherParticipant.User.Name;
                }
            }

            result.Add(roomView);
        }

        return await Task.FromResult(result);
    }

    public async Task<ChatRoom> CreateGroupRoom(CreateGroupRoomInput chatRoom)
    {
        await EnsureInitialized();

        var participants = new List<ChatRoomParticipant>();
        var roomId = Guid.NewGuid();

        // Add Creator as Admin
        var creator = await _userRepository.GetUserByEmail(chatRoom.CreatorEmail);
        if (creator != null)
        {
            participants.Add(new ChatRoomParticipant
            {
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
        var roomId = Guid.NewGuid();

        // Add Creator
        var creator = await _userRepository.GetUserByEmail(input.CreatorEmail);
        if (creator != null)
        {
            participants.Add(new ChatRoomParticipant
            {
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
            Name = null, // Private room has no name
            Type = RoomType.Private,
            CreatedAt = DateTime.Now,
            Participants = participants
        };

        ChatRooms.Add(newRoom);
        return await Task.FromResult(newRoom);
    }

    public async Task DeleteChatRoom(Guid id)
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