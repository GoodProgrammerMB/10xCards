using FlashCard.App.Models;
using FluentValidation;

namespace FlashCard.App.Validators;

public class RegisterValidator : AbstractValidator<RegisterModel>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidłowy format adresu email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Hasło jest wymagane")
            .MinimumLength(6).WithMessage("Hasło musi mieć co najmniej 6 znaków");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Potwierdzenie hasła jest wymagane")
            .Equal(x => x.Password).WithMessage("Hasła nie są identyczne");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<RegisterModel>.CreateWithOptions((RegisterModel)model, 
            x => x.IncludeProperties(propertyName)));
        
        if (result.IsValid)
            return Array.Empty<string>();
        
        return result.Errors.Select(e => e.ErrorMessage);
    };
} 