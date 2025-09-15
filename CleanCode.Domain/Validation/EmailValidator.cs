using FluentValidation;
using System.Text.RegularExpressions;

namespace CleanCode.Domain.Validation
{   
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(email => email)
                .NotEmpty()
                .WithMessage("O endereço de e-mail não pode estar vazio.")
                .MaximumLength(100)
                .WithMessage("O endereço de e-mail não pode ter mais de 100 caracteres.")
                .Must(BeValidEmail)
                .WithMessage("O endereço de e-mail fornecido não é válido.");
        }
      
        private bool BeValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;       
            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }
    }
}
