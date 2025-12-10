
using R2yChatSystem.Contracts.Enum;

namespace R2yChatSystem.Contracts.Inputs;

public class CreateMessageInput
{
    public Guid ChatRoomId { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? FileUrl { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public CreatePollInput? Poll { get; set; }
    
    public MessageType DetermineMessageType()
    {
        var messageType = MessageType.Text;

        if (Poll is not null)
            messageType = MessageType.Poll;

        if (FileUrl is not null)
            messageType = MessageType.File;

        if (this is { Content: not null, FileUrl: not null })
            messageType = MessageType.TextWithFile;
        
        return  messageType;
    }
}
