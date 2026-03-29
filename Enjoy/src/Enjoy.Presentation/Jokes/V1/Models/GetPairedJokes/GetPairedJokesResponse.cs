using Enjoy.Presentation.Jokes.V1.Models.Common;

namespace Enjoy.Presentation.Jokes.V1.Models.GetPairedJokes;

/// <summary>Collection of fused Chuck+Dad pairs.</summary>
/// <param name="Pairs">Paired rows.</param>
public sealed record GetPairedJokesResponse(IReadOnlyList<PairedJokeResponse> Pairs);

/// <summary>One paired row (Chuck + Dad + fused text); distinct from <see cref="JokeResponse"/> (listings with metadata).</summary>
/// <param name="Chuck">Chuck-side text or reference.</param>
/// <param name="Dad">Dad-side text or reference.</param>
/// <param name="Combined">Text fused by Gemini.</param>
public sealed record PairedJokeResponse(string Chuck, string Dad, string Combined);
