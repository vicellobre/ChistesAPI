using Enjoy.Presentation.Topics.V1.Models.Common;

namespace Enjoy.Presentation.Topics.V1.Models.ListTopics;

/// <summary>Collection of available topics.</summary>
/// <param name="Topics">Items with id and name.</param>
public sealed record ListTopicsResponse(IReadOnlyList<TopicResponse> Topics);
