using FluentValidation;
using VCIConsumer.Api.Models.Requests;

public class PaymentPostRequestValidator : AbstractValidator<PaymentPostRequest>
{
    public PaymentPostRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.StandardEntryClass)
            .IsInEnum().WithMessage("Standard entry class must be a valid value.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(30).WithMessage("Description must be 30 characters or fewer.");

        //TODO:  CTL Uncomment and implement these rules when the related models are available
        //RuleFor(x => x.Customer)
        //    .NotNull().WithMessage("Customer is required.")
        //    .SetValidator(new CustomerReferenceValidator());

        //RuleFor(x => x.BankAccount)
        //    .NotNull().WithMessage("Bank account is required.")
        //    .SetValidator(new BankAccountWithCheckValidator());
    }
}

public class CustomerReferenceValidator : AbstractValidator<PaymentPostWithTokenCustomerRequest>
{
    public CustomerReferenceValidator()
    {
        RuleFor(x => x.UUId)
            .NotEmpty().WithMessage("Customer UUID is required.");
    }
}

//TODO:  CTL Uncomment and implement this validator when the related model is available
//public class BankAccountWithCheckValidator : AbstractValidator<_BankAccountWithCheck>
//{
//    public BankAccountWithCheckValidator()
//    {
//        RuleFor(x => x.Check)
//            .NotNull().WithMessage("Check object is required.")
//            .SetValidator(new PaymentCheckValidator());
//    }
//}

//public class PaymentCheckValidator : AbstractValidator<PaymentCheck>
//{
//    public PaymentCheckValidator()
//    {
//        RuleFor(x => x.CheckNumber)
//            .NotEmpty().WithMessage("Check number is required.");

//        RuleFor(x => x.CheckImageFront)
//            .NotEmpty().WithMessage("Front image of the check is required (base64).");

//        RuleFor(x => x.CheckImageBack)
//            .NotEmpty().WithMessage("Back image of the check is required (base64).");
//    }
//}
