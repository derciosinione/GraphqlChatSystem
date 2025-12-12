using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using R2yChatSystem.Contracts.Types;
using R2yChatSystem.Helpers;
using R2yChatSystem.IRepository;

namespace R2yChatSystem.Graphql.Subscription;

[SubscriptionType]
public class ChatSubscriptions
{
    [Subscribe(With = nameof(CreateStream))]
    public async Task<Message> OnMessageReceivedAsync(
        [EventMessage] Guid messageId,
        [Service] IChatRepository chatRepository,
        CancellationToken cancellationToken)
    {
        return await chatRepository.GetMessageById(messageId, cancellationToken);
    }

    public async ValueTask<ISourceStream<Guid>> CreateStream(
        string currentUserEmail,
        [Service] ITopicEventReceiver receiver,
        CancellationToken cancellationToken)
    {
        var topicName = TopicsTypes.GetTopicName(TopicsTypes.User, currentUserEmail);
        return await receiver.SubscribeAsync<Guid>(topicName, cancellationToken);
    }
}