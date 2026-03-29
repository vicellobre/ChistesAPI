using Enjoy.Application.Abstractions.Messaging;

namespace Enjoy.Application.Maths.Queries.GetLeastCommonMultiple;

public sealed record GetLeastCommonMultipleQuery(
    IReadOnlyList<int> Numbers) : IQuery<GetLeastCommonMultipleQueryResponse>;
