namespace Enjoy.Presentation.Common;

/// <summary>Typed response for operations with no payload (e.g. update / delete).</summary>
/// <param name="Completed">Defaults to <c>true</c> when the operation finished without a domain error.</param>
public sealed record OperationCompletedResponse(bool Completed = true);
