public class Reaction : AggregateRoot
{
    public Reaction(UserId reactorId, UserId reactedOnId, MessageId messageId, ReactionValue reactionValue)
    {
        Id = new ReactionId(Guid.NewGuid().ToString());
        ReactorId = reactorId;
        ReactedOnId = reactedOnId;
        MessageId = messageId;
        ReactionValue = reactionValue;

        var reactionAdded = new ReactionAdded(Id, ReactorId, ReactedOnId, MessageId, ReactionValue);
        AddDomainEvent(reactionAdded);
    }

    public ReactionId Id { get; private set; }
    public UserId ReactorId { get; private set; }
    public UserId ReactedOnId { get; private set; }
    public MessageId MessageId { get; private set; }
    public ReactionValue ReactionValue { get; private set; }
}

public record ReactionId(string Value);

public record ReactionValue(string Value, string ReactionId, bool IsAnimated);

public record MessageId(string Value);
public record UserId(string Value);


public record ReactionAdded(ReactionId Id, UserId ReactorId, UserId ReactedOnId, MessageId MessageId, ReactionValue ReactionValue) : IDomainEvent;
