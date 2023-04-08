using AccountAPI.Entities;
using FluentValidation;

namespace AccountAPI.Models.Validators
{
    public class RegisterUserValidator :AbstractValidator<RegisterUserDto>
    {

        public RegisterUserValidator(AccountDbContext dbContext)
        {
            RuleFor(x => x.Email).
                NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.LastName).
              NotEmpty();


            RuleFor(x => x.Pesel).
              NotEmpty()
              .Length(11);
              


            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Length(9);
                

              

             
            RuleFor(x => x.Password).MinimumLength(7);

            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);

            RuleFor(x => x.Email).Custom((value, context) =>
            {
                var emailInUse = dbContext.Users.Any(u => u.Email == value);
                if (emailInUse)
                {
                    context.AddFailure("Email", "That email is taken");
                }

            });


        }
    }
}
