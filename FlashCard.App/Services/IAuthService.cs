using FlashCard.App.Models;

namespace FlashCard.App.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest model);
    Task<AuthResponse> LoginAsync(LoginRequest model);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
} 