
using System.Runtime.CompilerServices;
using HotChocolate.Subscriptions;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.Helpers;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Subscription;

[SubscriptionType]
public class ChatSubscriptions
{

    public async IAsyncEnumerable<Guid> CreateStream(string currentUserEmail,
        [Service] ITopicEventReceiver receiver, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var topicName = TopicsTypes.GetTopicName(TopicsTypes.User, currentUserEmail);
        
        var sourceStream = await receiver.SubscribeAsync<Guid>(topicName, cancellationToken);

        await Task.Delay(5000, cancellationToken);

        await foreach (var item in sourceStream.ReadEventsAsync().WithCancellation(cancellationToken))
        {
            yield return item;
        }
    }
    
    
    [Subscribe(With = nameof(CreateStream))]
    public async Task<Message?> OnMessageReceivedAsync(
        [EventMessage] Guid messageId,
        [Service] IChatRepository chatRepository,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await chatRepository.GetMessageById(messageId, cancellationToken);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
