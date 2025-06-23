using FluentValidation;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Models.Query;

public class CustomerListQueryValidator : AbstractValidator<CustomerListQuery>
{
    public CustomerListQueryValidator()
    {
        RuleFor(q => q.Sort)
            .MustBeInSortSet("created_at", "name");

        RuleFor(q => q.LimitPerPage)
            .NotNull().WithMessage("limit_per_page is required.")
            .GreaterThan(0).WithMessage("limit_per_page must be a positive integer.");

        RuleFor(q => q.PageNumber)
            .NotNull().WithMessage("page_number is required.")
            .GreaterThan(0).WithMessage("page_number must be a positive integer.");
    }
}
