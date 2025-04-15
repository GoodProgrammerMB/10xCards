using FlashCard.App.Models;
using FluentValidation;

namespace FlashCard.App.Validators;

public class LoginValidator : AbstractValidator<LoginModel>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidłowy format adresu email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Hasło jest wymagane");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<LoginModel>.CreateWithOptions((LoginModel)model, 
            x => x.IncludeProperties(propertyName)));
        
        if (result.IsValid)
            return Array.Empty<string>();
        
        return result.Errors.Select(e => e.ErrorMessage);
    };
} 