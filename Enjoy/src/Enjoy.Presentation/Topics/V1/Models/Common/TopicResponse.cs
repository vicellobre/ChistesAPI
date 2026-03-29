namespace Enjoy.Presentation.Topics.V1.Models.Common;

/// <summary>HTTP contract for a topic (listings).</summary>
/// <param name="Id">Topic id.</param>
/// <param name="Name">Display name.</param>
public sealed record TopicResponse(string Id, string Name);
