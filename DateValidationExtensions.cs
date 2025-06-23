using FluentValidation;

namespace VCIConsumer.Api.Extensions
{
    public static class DateValidationExtensions
    {
        public static IRuleBuilderOptions<T, DateTime?> MustBeBeforeOrEqualTo<T>(
            this IRuleBuilder<T, DateTime?> ruleBuilder,
            Func<T, DateTime?> otherDateSelector,
            string startDateFieldName,
            string endDateFieldName)
        {
            return ruleBuilder.Must((rootObject, startDate, context) =>
            {
                var endDate = otherDateSelector(rootObject);
                if (startDate.HasValue && endDate.HasValue)
                {
                    return startDate.Value <= endDate.Value;
                }
                return true; // If either date is null, consider it valid
            })
            .WithMessage($"{{PropertyName}} must be before or equal to {endDateFieldName}.");
        }
    }
}
