﻿using Microsoft.AspNetCore.Identity;
using RoadBlockTracker.Utilities.Interfaces;
using System.Security.Cryptography;

namespace RoadBlockTracker.Utilities.Security
{
    public class PasswordHasher: IPasswordHasher
    {
        private const int SaltSize = 16;  
        private const int KeySize = 32;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private const char Delimiter = ';';

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                _hashAlgorithmName,
                KeySize);
            return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public bool Verify(string passwordHash, string inputPassword)
        {
            var elements = passwordHash.Split(Delimiter);
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);
            var hashInput = Rfc2898DeriveBytes.Pbkdf2(
                inputPassword,
                salt,
                Iterations,
                _hashAlgorithmName,
                KeySize);
            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}
