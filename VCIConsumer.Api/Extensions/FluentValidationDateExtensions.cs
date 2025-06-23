using FluentValidation;

namespace VCIConsumer.Api.Extensions;

public static class FluentValidationDateExtensions
{
    public static IRuleBuilderOptions<T, DateTime?> MustBeBeforeOrEqualTo<T>(
        this IRuleBuilder<T, DateTime?> ruleBuilder,
        Func<T, DateTime?> upperBoundSelector,
        string lowerName,
        string upperName)
    {
        return ruleBuilder
            .Must((rootObject, lower) =>
            {
                var upper = upperBoundSelector(rootObject);
                return !lower.HasValue || !upper.HasValue || lower <= upper;
            })
            .WithMessage($"{lowerName} must be earlier than or equal to {upperName}.");
    }
}
