using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalterApi.Core.DTO_s.User;

namespace WalterApi.Core.Validations
{
    public class UpdateUserValidation : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidation()
        {
            RuleFor(r => r.FirstName).NotEmpty().MaximumLength(64).MinimumLength(2);
            RuleFor(r => r.LastName).NotEmpty().MaximumLength(64).MinimumLength(2);
            RuleFor(r => r.Email).NotEmpty().EmailAddress().MaximumLength(128);
            RuleFor(r => r.Role).NotEmpty();
        }
    }
}
