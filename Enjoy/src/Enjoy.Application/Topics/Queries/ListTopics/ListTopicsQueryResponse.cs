namespace Enjoy.Application.Topics.Queries.ListTopics;

public sealed record ListTopicsQueryResponse(IReadOnlyList<TopicListItem> Topics);

public sealed record TopicListItem(string Id, string Name);
