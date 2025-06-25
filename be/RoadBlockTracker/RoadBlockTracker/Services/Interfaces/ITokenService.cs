using EFDataAccessLibrary.Models;
using System.Security.Claims;

namespace RoadBlockTracker.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
