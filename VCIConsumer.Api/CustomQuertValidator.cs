using FluentValidation;
using VCIConsumer.Api.Models.Query;

public class CustomerQueryValidator : AbstractValidator<CustomerListQuery>
{
    public CustomerQueryValidator()
    {
        RuleFor(q => q.Sort)
            .Must(val => string.IsNullOrWhiteSpace(val) ||
                         new[] { "created_at", "name", "-created_at", "-name" }.Contains(val))
            .WithMessage("Sort must be one of: 'created_at', 'name', '-created_at', or '-name'.");

        RuleFor(q => q.Limit_Per_Page)
            .Must(val => string.IsNullOrWhiteSpace(val) || int.TryParse(val, out var parsed) && parsed > 0)
            .WithMessage("Limit_Per_Page must be a positive integer.");

        RuleFor(q => q.Page_Number)
            .Must(val => string.IsNullOrWhiteSpace(val) || int.TryParse(val, out var parsed) && parsed > 0)
            .WithMessage("Page_Number must be a positive integer.");
    }
}
