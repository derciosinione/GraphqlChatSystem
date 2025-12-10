using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using R2yChatSystem.Contracts.Enum;
using R2yChatSystem.Contracts.Inputs;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.IRepository;
using R2yChatSystem.Mappers;
using R2yChatSystem.Model;

namespace R2yChatSystem.Repository;

public class ChatRepository : IChatRepository
{
    private readonly string _connectionString;
    private readonly IUserRepository _userRepository;

    public ChatRepository(IConfiguration configuration, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        InitializeDatabase().Wait();
    }

    private async Task InitializeDatabase()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var tableExists = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ChatRooms'");

        if (tableExists == 0)
        {
            await connection.ExecuteAsync("""
                                              CREATE TABLE ChatRooms
                                              (
                                                  Id           UNIQUEIDENTIFIER PRIMARY KEY,
                                                  Type         NVARCHAR(20)  NOT NULL,
                                                  Name         NVARCHAR(255) NULL,
                                                  Participants NVARCHAR(MAX) NULL,
                                                  Messages     NVARCHAR(MAX) NULL,
                                                  CreatedAt    DATETIME2     NOT NULL
                                              )
                                          """);
        }
    }

    public async Task<List<ChatRoom>> GetAllChatRooms()
    {
        await using var connection = new SqlConnection(_connectionString);
        var rooms = await connection.QueryAsync<ChatRoomDbModel>("SELECT * FROM ChatRooms");
        return rooms.ToResponse();
    }

    public async Task<ChatRoom> GetChatRoomById(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var room = await connection.QueryFirstOrDefaultAsync<ChatRoomDbModel>(
            "SELECT * FROM ChatRooms WHERE Id = @Id", new { Id = id });

        return room?.ToResponse() ?? throw new Exception("Chat room not found");
    }

    public async Task<List<ChatRoom>> GetAllChatRoomByUserEmail(string userEmail)
    {
        await using var connection = new SqlConnection(_connectionString);

        // Using OPENJSON to filter by user email in Participants JSON array
        // Participants is ChatRoomParticipant[] -> { "UserEmail": "...", ... }
        const string sql = """
                               SELECT * FROM ChatRooms
                               WHERE EXISTS (
                                   SELECT 1 
                                   FROM OPENJSON(Participants) WITH (UserEmail NVARCHAR(255) '$.User.Email') 
                                   WHERE UserEmail = @UserEmail
                               )
                           """;

        var rooms = await connection.QueryAsync<ChatRoomDbModel>(sql, new { UserEmail = userEmail });

        var result = new List<ChatRoom>();

        foreach (var room in rooms.ToResponse())
        {
            var roomView = room.ShallowCopy();

            if (room.Type == RoomType.Private)
            {
                var otherParticipant = room.Participants.FirstOrDefault(p =>
                    !p.User.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase));

                if (otherParticipant?.User != null)
                    roomView.Name = otherParticipant.User.Name;
            }

            result.Add(roomView);
        }

        return result;
    }

    public async Task<ChatRoom> CreateGroupRoom(CreateGroupRoomInput input)
    {
        var participants = new List<ChatRoomParticipant>();

        var creatorUser = await _userRepository.GetUserByEmail(input.CreatorEmail) ??
                          throw new Exception("Creator user not found");

        // Add Creator as Admin
        participants.Add(ChatRoomParticipant.Add(creatorUser, Role.Admin));

        //TODO: Implement a method in _userRepository to Get Users by Email List, send list of emails and return all users so that do not have to call db in loop
        foreach (var email in input.ParticipantEmails.Where(email => email != input.CreatorEmail))
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user != null) participants.Add(ChatRoomParticipant.Add(user));
        }

        return await CreateChatRoomWrapper(roomType: RoomType.Group, input.Name, participants: participants);
    }

    public async Task<ChatRoom> CreatePrivateRoom(CreatePrivateRoomInput input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input.CreatorEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(input.UserEmail);

        if (input.UserEmail == input.CreatorEmail)
            throw new Exception("The destination user can not be the same as creator.");

        if (await CheckIfPrivateRoomExists(input.CreatorEmail, input.UserEmail))
            throw new Exception("A private chat between these users already exists.");
        
        var participants = new List<ChatRoomParticipant>();

        var creatorUser = await _userRepository.GetUserByEmail(input.CreatorEmail) ??
                          throw new Exception("Creator user not found");
        var destinationUser = await _userRepository.GetUserByEmail(input.UserEmail) ??
                              throw new Exception("Destination user not found");

        participants.Add(ChatRoomParticipant.Add(creatorUser));
        participants.Add(ChatRoomParticipant.Add(destinationUser));

        return await CreateChatRoomWrapper(participants: participants);
    }
    
    public async Task<List<Message>> GetAllMessagesByChatRoomId(Guid chatRoomId)
    {
        await using var connection = new SqlConnection(_connectionString);
        var json = await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT Messages FROM ChatRooms WHERE Id = @Id", new { Id = chatRoomId });

        if (string.IsNullOrEmpty(json)) return [];
        return JsonSerializer.Deserialize<List<Message>>(json) ?? [];
    }

    public async Task<Message> SendMessage(CreateMessageInput input)
    {
        var sender = await _userRepository.GetUserByEmail(input.SenderEmail)
                     ?? throw new Exception("Sender user not found");

        var room = await GetChatRoomById(input.ChatRoomId);

        var isValidParticipant =
            room.Participants.Any(x => x.User.Email.Equals(input.SenderEmail, StringComparison.OrdinalIgnoreCase));
        
        if (!isValidParticipant)
            throw  new Exception("Sender user not found in the chat room.");

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
            Type = input.DetermineMessageType(),
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

        await UpdateRoomMessages(input.ChatRoomId, room.Messages);

        return newMessage;
    }

    public async Task<Message> VoteOnPoll(VotePollInput input)
    {
        await using var connection = new SqlConnection(_connectionString);

        // Find the room containing the message using OPENJSON
        const string sql = """
                               SELECT Top 1 * FROM ChatRooms 
                               WHERE EXISTS (
                                   SELECT 1 
                                   FROM OPENJSON(Messages) WITH (Id UNIQUEIDENTIFIER '$.Id') 
                                   WHERE Id = @MessageId
                               )
                           """;

        var roomDb = await connection.QueryFirstOrDefaultAsync<ChatRoomDbModel>(sql, new { input.MessageId })
                     ?? throw new Exception("Poll not found.");

        var room = roomDb.ToResponse();
        var message = room.Messages.FirstOrDefault(m => m.Id == input.MessageId)
                      ?? throw new Exception("Message not found");

        if (message.Poll == null) throw new Exception("Poll not found.");

        if (message.Poll.ExpiresAt.HasValue && message.Poll.ExpiresAt < DateTime.Now)
            throw new Exception("Poll has expired.");

        var option = message.Poll.Options.FirstOrDefault(o => o.Id == input.OptionId)
                     ?? throw new Exception("Invalid poll option.");

        var alreadyVoted = message.Poll.Options.Any(o => o.Votes.Any(v => v.UserEmail == input.UserEmail));

        if (!message.Poll.IsMultipleChoice && alreadyVoted)
        {
            foreach (var opt in message.Poll.Options)
            {
                var existingVote = opt.Votes.FirstOrDefault(v => v.UserEmail == input.UserEmail);
                if (existingVote != null)
                {
                    opt.Votes.Remove(existingVote);
                }
            }
        }

        if (option.Votes.All(v => v.UserEmail != input.UserEmail))
        {
            option.Votes.Add(new PollVote { UserEmail = input.UserEmail });
        }
        else
        {
            var existing = option.Votes.First(v => v.UserEmail == input.UserEmail);
            option.Votes.Remove(existing);
        }

        await UpdateRoomMessages(room.Id, room.Messages);

        return message;
    }

    public async Task DeleteChatRoom(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("DELETE FROM ChatRooms WHERE Id = @Id", new { Id = id });
    }

    #region  Helpers

     private async Task UpdateRoomMessages(Guid roomId, List<Message> messages)
    {
        var json = JsonSerializer.Serialize(messages);
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE ChatRooms SET Messages = @Messages WHERE Id = @Id",
            new { Messages = json, Id = roomId });
    }

    private async Task<ChatRoom> CreateChatRoomWrapper(RoomType roomType = RoomType.Private, string? roomName = null,
        List<ChatRoomParticipant>? participants = null!)
    {
        participants ??= [];

        var newRoom = new ChatRoom
        {
            Name = roomName,
            Type = roomType,
            CreatedAt = DateTime.Now,
            Participants = participants
        };

        var dbModel = new
        {
            newRoom.Id,
            Type = newRoom.Type.ToString(),
            newRoom.Name,
            Participants = JsonSerializer.Serialize(newRoom.Participants),
            Messages = JsonSerializer.Serialize(new List<Message>()),
            newRoom.CreatedAt
        };

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("""
                                          INSERT INTO ChatRooms (Id, Type, Name, Participants, Messages, CreatedAt)
                                          VALUES (@Id, @Type, @Name, @Participants, @Messages, @CreatedAt)
                                      """, dbModel);

        return newRoom;
    }
    
    private async Task<bool> CheckIfPrivateRoomExists(string firstEmail, string secondEmail)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = """
                               SELECT COUNT(1) 
                               FROM ChatRooms 
                               WHERE Type = 'Private' 
                               AND EXISTS (
                                   SELECT 1 FROM OPENJSON(Participants) WITH (UserEmail NVARCHAR(255) '$.User.Email') WHERE UserEmail = @FirstEmail
                               )
                               AND EXISTS (
                                   SELECT 1 FROM OPENJSON(Participants) WITH (UserEmail NVARCHAR(255) '$.User.Email') WHERE UserEmail = @SecondEmail
                               )
                           """;

        var count = await connection.ExecuteScalarAsync<int>(sql, new { FirstEmail = firstEmail, SecondEmail = secondEmail });
        return count > 0;
    }
    #endregion
}