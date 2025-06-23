using FluentValidation;

namespace VCIConsumer.Api.Extensions;

public static class FluentValidationSortExtensions
{
    public static IRuleBuilderOptions<T, string?> MustBeInSortSet<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        params string[] validFields)
    {
        var allowed = validFields.SelectMany(f => new[] { f, $"-{f}" }).ToHashSet();

        return ruleBuilder
            .Must(val => string.IsNullOrWhiteSpace(val) || allowed.Contains(val))
            .WithMessage($"Sort must be one of: {string.Join(", ", allowed.Select(f => $"'{f}'"))}.");
    }
}