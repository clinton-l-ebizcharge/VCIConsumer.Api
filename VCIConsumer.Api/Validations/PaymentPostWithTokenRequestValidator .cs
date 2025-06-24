using FluentValidation;
using VCIConsumer.Api.Models.Requests;

public class PaymentPostWithTokenRequestValidator : AbstractValidator<PaymentPostWithTokenRequest>
{
    public PaymentPostWithTokenRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.StandardEntryClass)
            .IsInEnum().WithMessage("Standard entry class must be a valid value.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(30).WithMessage("Description must be 30 characters or fewer.");

        RuleFor(x => x.Customer)
            .NotNull().WithMessage("Customer is required.")
            .SetValidator(new PaymentPostWithTokenCustomerRequestValidator());

        RuleFor(x => x.Check)
            .NotNull().WithMessage("Check is required.")
            .SetValidator(new PaymentPostWithTokenCheckRequestValidator());
    }
}

public class PaymentPostWithTokenCustomerRequestValidator : AbstractValidator<PaymentPostWithTokenCustomerRequest>
{
    public PaymentPostWithTokenCustomerRequestValidator()
    {
        RuleFor(x => x.UUId)
            .NotEmpty().WithMessage("Customer UUID is required.");
    }
}

public class PaymentPostWithTokenCheckRequestValidator : AbstractValidator<PaymentPostWithTokenCheckRequest>
{
    public PaymentPostWithTokenCheckRequestValidator()
    {
        RuleFor(x => x.CheckNumber)
            .NotEmpty().WithMessage("Check number is required.");

        RuleFor(x => x.CheckImageFront)
            .NotEmpty().WithMessage("Front check image (base64) is required.");

        RuleFor(x => x.CheckImageBack)
            .NotEmpty().WithMessage("Back check image (base64) is required.");
    }
}
