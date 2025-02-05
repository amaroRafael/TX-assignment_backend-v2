namespace TX.RMC.BusinessLogic.Security.Crypt;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

internal static class Pbkdf2Hasher
{
    public static string ComputeHash(string password, byte[] salt)
    {
        return Convert.ToBase64String(
          KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 3000,
            numBytesRequested: 512 / 8));
    }

    public static byte[] GenerateRandomSalt()
    {
        byte[] salt = new byte[256 / 8];

        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(salt);

        return salt;
    }
}
