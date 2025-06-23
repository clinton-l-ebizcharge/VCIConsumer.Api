using FluentValidation;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Models.Query;

public class PaymentListQueryValidator : AbstractValidator<PaymentListQuery>
{
    public PaymentListQueryValidator()
    {
        RuleFor(q => q.Sort)
            .MustBeInSortSet("created_at", "status", "amount", "name");

        RuleFor(q => q.LimitPerPage)
            .GreaterThan(0)
            .WithMessage("limit_per_page must be a positive integer.");

        RuleFor(q => q.PageNumber)
            .GreaterThan(0)
            .WithMessage("page_number must be a positive integer.");

        // Date range comparisons using your earlier reusable date rule if available
        RuleFor(q => q.CreatedAtGte)
            .MustBeBeforeOrEqualTo(q => q.CreatedAtLte, "created_at[gte]", "created_at[lte]");
        RuleFor(q => q.OriginatedAtGte)
            .MustBeBeforeOrEqualTo(q => q.OriginatedAtLte, "originated_at[gte]", "originated_at[lte]");
        RuleFor(q => q.SettledAtGte)
            .MustBeBeforeOrEqualTo(q => q.SettledAtLte, "settled_at[gte]", "settled_at[lte]");
        RuleFor(q => q.ReturnedAtGte)
            .MustBeBeforeOrEqualTo(q => q.ReturnedAtLte, "returned_at[gte]", "returned_at[lte]");
    }
}
