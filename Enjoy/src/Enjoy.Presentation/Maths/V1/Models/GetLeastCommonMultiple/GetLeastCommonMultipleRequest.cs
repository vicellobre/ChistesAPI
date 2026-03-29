using Microsoft.AspNetCore.Mvc;

namespace Enjoy.Presentation.Maths.V1.Models.GetLeastCommonMultiple;

/// <summary>Comma-separated integers for LCM.</summary>
/// <param name="Numbers">Example: <c>4,6,8</c>.</param>
public sealed record GetLeastCommonMultipleRequest(
    [property: FromQuery(Name = "numbers")] string Numbers);
