using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccessLibrary.Models.Auth
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; } // in seconds

        // Success constructor
        public static AuthResponse SuccessResponse(string accessToken, string refreshToken, int expiresIn)
        {
            return new AuthResponse
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn
            };
        }

        // Failure constructor
        public static AuthResponse FailResponse(string message)
        {
            return new AuthResponse
            {
                Success = false,
                Message = message
            };
        }
    }
}
