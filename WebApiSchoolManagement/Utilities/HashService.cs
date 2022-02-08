using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiSchoolManagement.DTO.Hash;

namespace WebApiSchoolManagement.Utilities
{
    public class HashService
    {
        public HashDTO Hash(string text)
        {
            var salt = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Hash(text, salt);
        }

        public HashDTO Hash(string text, byte[] salt)
        {
            var derivedKey = KeyDerivation
                .Pbkdf2(
                    password: text,
                    salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 32);

            var hash = Convert.ToBase64String(derivedKey);

            return new HashDTO()
            {
                Hash = hash,
                Salt = salt
            };
        }
    }
}
