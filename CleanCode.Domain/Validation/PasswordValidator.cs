using FluentValidation;

namespace CleanCode.Domain.Validation
{   
    public class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator()
        {
            RuleFor(password => password)
                .NotEmpty()
                .WithMessage("A senha não pode estar vazia.")
                .MinimumLength(6)
                .WithMessage("A senha deve conter no mínimo 8 caracteres.")
                .Matches(@"[A-Z]+")
                .WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
                .Matches(@"[a-z]+")
                .WithMessage("A senha deve conter pelo menos uma letra minúscula.")
                .Matches(@"[0-9]+")
                .WithMessage("A senha deve conter pelo menos um número.")
                .Matches(@"[\!\?\*\.\@\#\$\%\^\&\+\=]+");               
        }
    }
}
