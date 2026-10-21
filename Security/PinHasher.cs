using System;
using System.Security.Cryptography;

namespace Student_Att.Security
{
    // Turns a PIN into a hash that is safe to store, and checks a PIN against it.
    // Stored format:  iterations.salt.hash   (salt and hash in Base64)
    public static class PinHasher
    {
        private const int SaltSize = 16;      // bytes
        private const int HashSize = 32;      // bytes
        private const int Iterations = 100000;

        public static string HashPin(string pin)
        {
            // A new random salt for every user, so two equal PINs give different hashes
            byte[] salt = new byte[SaltSize];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            byte[] hash = CreateHash(pin, salt, Iterations);

            return Iterations + "." + Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public static bool VerifyPin(string pin, string storedHash)
        {
            if (string.IsNullOrEmpty(pin) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            string[] parts = storedHash.Split('.');
            int iterations;
            if (parts.Length != 3 || !int.TryParse(parts[0], out iterations))
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expectedHash = Convert.FromBase64String(parts[2]);

            // Hash the typed PIN with the SAME salt and compare
            byte[] actualHash = CreateHash(pin, salt, iterations);
            return SecureCompare.AreEqual(expectedHash, actualHash);
        }

        // PBKDF2: runs SHA-256 many times on purpose, so guessing PINs is slow
        private static byte[] CreateHash(string pin, byte[] salt, int iterations)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pin, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }
    }
}
