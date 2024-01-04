using DataAccess.Dto.Request;
using FluentValidation;

namespace GlobalAccess.Validators
{
    public class SMSSendValidator : AbstractValidator<SMSSendReqDto>
    {
        public SMSSendValidator() 
        {
            RuleFor(d => d.accId).NotNull().GreaterThan(0).LessThanOrEqualTo(4).WithMessage("AccountId must be greater than 0 and less than 5");
            RuleFor(d => d.pNumber).NotNull().NotEmpty().WithMessage("Phone number is required");
            RuleFor(d => d.pNumber).Must(PNumberLength).WithMessage("Phone number should contain only 10 digits without country code");
            RuleFor(d => d.message).NotNull().NotEmpty().WithMessage("SMS content is required");
        }
        
        private bool PNumberLength(string pNumber)
        {
            if (Convert.ToString(pNumber).Length > 10)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
