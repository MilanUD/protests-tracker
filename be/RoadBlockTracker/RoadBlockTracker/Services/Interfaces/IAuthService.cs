using EFDataAccessLibrary.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace RoadBlockTracker.Services.Interfaces
{
    public interface IAuthService
    {
        // Core methods (already implemented)
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(int userId);
        Task<AuthResponse> RegisterAsync(RegisterRequest request, string confirmationBaseUrl);
        public Task<bool> ConfirmEmailAsync(int userId, string token);
    }
}
