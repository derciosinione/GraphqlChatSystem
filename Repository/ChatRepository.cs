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
                        UserEmail = "ana.silva@example.com",
                        Role = Role.Admin,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("ana.silva@example.com")
                    },
                    new ChatRoomParticipant
                    {
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
                        UserEmail = "ana.silva@example.com",
                        Role = Role.Admin,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("ana.silva@example.com")
                    },
                    new ChatRoomParticipant
                    {
                        UserEmail = "bruno.costa@example.com",
                        Role = Role.Member,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("bruno.costa@example.com")
                    },
                    new ChatRoomParticipant
                    {
                        UserEmail = "ze.luis@example.com",
                        Role = Role.Member,
                        JoinedAt = DateTime.Now,
                        User = await _userRepository.GetUserByEmail("ze.luis@example.com")
                    },
                    new ChatRoomParticipant
                    {
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
        var room = ChatRooms.FirstOrDefault(c => c.Id == id)
                   ?? throw new Exception("Chat room not found");
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
    public async Task<List<Message>> GetAllMessagesByChatRoomId(Guid chatRoomId)
    {
        await EnsureInitialized();
        var room = ChatRooms.FirstOrDefault(c => c.Id == chatRoomId);
        if (room == null) return [];
        return await Task.FromResult(room.Messages ?? []);
    }

    public async Task<Message> SendMessage(CreateMessageInput input)
    {
        await EnsureInitialized();

        var sender = await _userRepository.GetUserByEmail(input.SenderEmail) ?? throw new Exception("Sender user not found");

        var room = ChatRooms.FirstOrDefault(c => c.Id == input.ChatRoomId)
                   ?? throw new Exception("Chat room not found");
        
        Message? replyToMessage = null;
        if (input.ReplyToMessageId.HasValue)
        {
            replyToMessage = room.Messages.FirstOrDefault(m => m.Id == input.ReplyToMessageId.Value);
        }

        Poll? poll = null;
        if (input.Poll != null)
        {
            if (string.IsNullOrWhiteSpace(input.Poll.Question) || input.Poll.Options.Count < 2)
                throw new Exception("Poll must have a question and at least 2 options.");

            poll = new Poll
            {
                Question = input.Poll.Question,
                IsMultipleChoice = input.Poll.IsMultipleChoice,
                ExpiresAt = input.Poll.ExpiresAt,
                Options = input.Poll.Options.Select(o => new PollOption { Text = o }).ToList()
            };
        }
        
        var newMessage = new Message
        {
            Type =  input.DetermineMessageType(),
            ChatRoomId = input.ChatRoomId,
            Sender = sender,
            Content = input.Content,
            FileUrl = input.FileUrl,
            ReplyToMessage = replyToMessage,
            Poll = poll,
            SentAt = DateTime.Now
        };

        room.Messages ??= [];
        room.Messages.Add(newMessage);

        return await Task.FromResult(newMessage);
    }
    
    public async Task<Message> VoteOnPoll(VotePollInput input)
    {
        await EnsureInitialized();
        // Locate the message containing the poll
        // In a real DB we'd query by MessageId directly. Here we might need to iterate rooms or assume we know the room?
        // Since VotePollInput doesn't have ChatRoomId, we have to search all rooms or enforce RoomId in Input.
        // For efficiency in this mock, let's assume valid ID finds it. Traversing all rooms is okay for mock.

        Message? message = null;
        foreach (var room in ChatRooms)
        {
            if (room.Messages == null) continue;
            message = room.Messages.FirstOrDefault(m => m.Id == input.MessageId);
            if (message != null) break;
        }

        if (message == null || message.Poll == null) throw new Exception("Poll not found.");

        if (message.Poll.ExpiresAt.HasValue && message.Poll.ExpiresAt < DateTime.Now)
            throw new Exception("Poll has expired.");

        var option = message.Poll.Options.FirstOrDefault(o => o.Id == input.OptionId)
                     ?? throw new Exception("Invalid poll option.");

        // Check if user already voted on this poll
        var alreadyVoted = message.Poll.Options.Any(o => o.Votes.Any(v => v.UserEmail == input.UserEmail));

        if (!message.Poll.IsMultipleChoice && alreadyVoted)
        {
            // If single choice and already voted, maybe we change vote? Or throw?
            // Let's implement toggle/change behavior: remove old vote first.
            foreach (var opt in message.Poll.Options)
            {
                var existingVote = opt.Votes.FirstOrDefault(v => v.UserEmail == input.UserEmail);
                if (existingVote != null)
                {
                    opt.Votes.Remove(existingVote);
                }
            }
        }

        // Add new vote (if not already voted for this specific option, to avoid duplicates)
        if (option.Votes.All(v => v.UserEmail != input.UserEmail))
        {
            option.Votes.Add(new PollVote { UserEmail = input.UserEmail });
        }
        else
        {
            // Toggle off if clicking same option? Common UI pattern.
            var existing = option.Votes.First(v => v.UserEmail == input.UserEmail);
            option.Votes.Remove(existing);
        }

        return await Task.FromResult(message);
    }
}