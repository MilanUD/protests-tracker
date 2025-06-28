using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Models;
using EFDataAccessLibrary.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoadBlockTracker.Services.Interfaces;
using RoadBlockTracker.Utilities.Interfaces;
using RoadBlockTracker.Utilities.Security;
using System.Net;
using System.Security.Claims;

namespace RoadBlockTracker.Services.Implementations
{
    public class AuthService: IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;

        public AuthService(
            AppDbContext context,
        ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IEmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !_passwordHasher.Verify(user.PasswordHash, password))
                return AuthResponse.FailResponse("Invalid credentials");

            if (!user.IsEmailConfirmed)
                return AuthResponse.FailResponse("Email not confirmed");

            var tokens = await GenerateTokens(user);
            return AuthResponse.SuccessResponse(
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresIn);
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
                return AuthResponse.FailResponse("Invalid refresh token");

            var tokens = await GenerateTokens(user);
            return AuthResponse.SuccessResponse(
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresIn);
        }

        public async Task LogoutAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<TokenPair> GenerateTokens(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new TokenPair(accessToken, refreshToken, 900); 
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string confirmationBaseUrl)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return AuthResponse.FailResponse("Email is already registered.");

            var hashedPassword = _passwordHasher.Hash(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                Name = request.FirstName,
                LastName = request.LastName,
                IsEmailConfirmed = false,
                Role = EFDataAccessLibrary.Models.Role.USER,
                Username = request.Username
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = Guid.NewGuid().ToString(); 
            user.EmailConfirmationToken = token; 
            user.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddDays(1); 

            await _context.SaveChangesAsync();

            // Include token in the link
            var confirmationLink = $"{confirmationBaseUrl}?userId={user.Id}&token={WebUtility.UrlEncode(token)}";
            await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);
            var tokens = await GenerateTokens(user);

            return AuthResponse.SuccessResponse(
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresIn);
        }

        public async Task<bool> ConfirmEmailAsync(int userId, string token)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsEmailConfirmed || user.EmailConfirmationToken != token ||
                user.EmailConfirmationTokenExpiry < DateTime.UtcNow)
                return false;

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            await _context.SaveChangesAsync();
            return true;
        }


    }
    public record TokenPair(string AccessToken, string RefreshToken, int ExpiresIn);

}
