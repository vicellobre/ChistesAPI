using Microsoft.AspNetCore.Mvc;

namespace Enjoy.Presentation.Maths.V1.Models.GetNextNumber;

/// <summary>Query <c>?number=</c>. <see cref="Number"/> is nullable for model validation and to detect a missing parameter.</summary>
/// <param name="Number">Current term in the sequence. Example: <c>7</c>.</param>
public sealed record GetNextNumberRequest(
    [property: FromQuery(Name = "number")] int? Number);
