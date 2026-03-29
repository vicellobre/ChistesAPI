using FluentValidation;

namespace Enjoy.Application.Maths.Queries.GetLeastCommonMultiple;

internal sealed class GetLeastCommonMultipleQueryValidator
    : AbstractValidator<GetLeastCommonMultipleQuery>
{
    public GetLeastCommonMultipleQueryValidator()
    {
        RuleFor(x => x.Numbers)
            .NotEmpty()
            .WithMessage("At least one number must be provided.");

        RuleForEach(x => x.Numbers)
            .GreaterThan(0)
            .WithMessage("All numbers must be positive integers.");
    }
}
