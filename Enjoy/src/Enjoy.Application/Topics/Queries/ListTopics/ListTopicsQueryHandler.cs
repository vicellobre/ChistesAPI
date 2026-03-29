using Enjoy.Application.Abstractions.Messaging;
using Enjoy.Domain.Shared.Results;
using Enjoy.Domain.Topics.Repositories;

namespace Enjoy.Application.Topics.Queries.ListTopics;

internal sealed class ListTopicsQueryHandler(ITopicRepository topicRepository)
    : IQueryHandler<ListTopicsQuery, ListTopicsQueryResponse>
{
    public async Task<Result<ListTopicsQueryResponse>> Handle(
        ListTopicsQuery request,
        CancellationToken cancellationToken)
    {
        var topics = await topicRepository.ListAsync();
        var items = topics
            .Select(t => new TopicListItem(t.Id, t.Name))
            .OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Result.Success(new ListTopicsQueryResponse(items));
    }
}
