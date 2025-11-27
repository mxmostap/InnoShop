using FluentValidation;

namespace ProductManagement.Application.Validators.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, decimal> DecimalPrecision<T>(
            this IRuleBuilder<T, decimal> ruleBuilder, 
            int maxScale)
        {
            return ruleBuilder.Must(value => 
            {
                var bits = decimal.GetBits(value);
                var scale = (bits[3] >> 16) & 0x7F;
                return scale <= maxScale;
            });
        }
}