using System.ComponentModel.DataAnnotations;

namespace FlashCard.Api.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Email jest wymagany")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane")]
    [StringLength(100, ErrorMessage = "Hasło musi mieć co najmniej {2} znaków.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

public class LoginModel
{
    [Required(ErrorMessage = "Email jest wymagany")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane")]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public bool Successful { get; set; }
    public string? Error { get; set; }
    public string? Token { get; set; }
    public UserDto? User { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
} 