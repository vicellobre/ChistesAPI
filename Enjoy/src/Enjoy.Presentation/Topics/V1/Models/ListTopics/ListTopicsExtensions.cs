using Enjoy.Application.Topics.Queries.ListTopics;
using Enjoy.Presentation.Topics.V1.Models;
using Enjoy.Presentation.Topics.V1.Models.Common;

namespace Enjoy.Presentation.Topics.V1.Models.ListTopics;

public static class ListTopicsExtensions
{
    public static ListTopicsQuery ToQuery(this ListTopicsRequest _) =>
        new();

    public static ListTopicsResponse ToResponse(this ListTopicsQueryResponse response) =>
        new(response.Topics.Select(t => new TopicResponse(t.Id, t.Name)).ToList());
}
