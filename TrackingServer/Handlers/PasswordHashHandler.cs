using System.Security.Cryptography;

namespace TrackingServer.Handlers
{
    public class PasswordHashHandler
    {
        private static RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

        // Hash a password with a salt
        public static string HashPassword(string password, out byte[] salt)
        {
            // Generate a cryptographic random number for the salt
            salt = new byte[16];
            _randomNumberGenerator.GetBytes(salt);

            // Create the Rfc2898DeriveBytes and get the hash value
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                // Combine the salt and password bytes for later use
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                // Convert the result to a base64 string
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Verify a password against a stored hash and salt
        public static bool VerifyPassword(string password, string storedHash)
        {
            // Get the hash bytes from the stored hash
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            // Get the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            // Hash the incoming password with the stored salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                // Compare the results
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
