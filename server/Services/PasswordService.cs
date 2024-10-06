using System;
using System.Security.Cryptography;

namespace server.Services
{
    public class PasswordService
    {
        // Method to hash the password using PBKDF2
        public string HashPassword(string password)
        {
            // Generate a salt
            byte[] salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(salt);

            // Use PBKDF2 algorithm to hash the password with the salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine the salt and the hash for storage
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Return the base64-encoded string of the combined salt and hash
            return Convert.ToBase64String(hashBytes);
        }

        // Method to verify the password
        public bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // Extract the bytes
            byte[] hashBytes = Convert.FromBase64String(storedPassword);

            // Get the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Compute the hash of the entered password
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Compare the results
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
