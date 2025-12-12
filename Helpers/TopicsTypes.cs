namespace R2yChatSystem.Helpers;

public static class TopicsTypes
{
    public const string User = "User";
    public const string Group = "Group";
    public const string Private = "Private";


    public static string GetTopicName(string topicName, string groupName)
    {
        return $"{topicName}_{groupName}_MESSAGE";
    }
}