using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Maths.Queries.GetNextNumber;

public sealed record GetNextNumberQuery(int Number) : IQuery<GetNextNumberQueryResponse>;
