using Queue_Management_System.Interfaces;
using System.Security.Cryptography;

namespace Queue_Management_System.Services;

public class QueueService : IAccountService
{
    public QueueService()
    {
         
    }
    public string HashPassword(string password)
    {
        /// <summary>
        /// A salt size of 16 bytes is commonly used because it provides a good balance between security and storage efficiency.
        /// It's long enough to be unique for each user but not so long that it significantly increases the size of the stored hash. 
        /// The salt is crucial for preventing rainbow table attacks and ensuring that even if two users have the same password, their hashes will be different 
        /// </summary>
        const int saltSize = 16;
        const int hashSize = 24;
        const int iterations = 100000;

        // Generate a random salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[saltSize]);

        // Hash the password with the salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        byte[] hash = pbkdf2.GetBytes(hashSize);

        // Combine the salt and hash
        byte[] hashBytes = new byte[saltSize + hashSize];
        Array.Copy(salt, 0, hashBytes, 0, saltSize);
        Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

        // Convert the combined salt and hash to a Base64 string
        return Convert.ToBase64String(hashBytes);
    }
    public bool VerifyPassword(string password, string storedHash)
    {
        const int saltSize = 16;
        const int hashSize = 24;
        const int iterations = 100000;

        // Convert the stored hash from Base64 to bytes
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        // Extract the salt and the hash from the stored hash bytes
        byte[] salt = new byte[saltSize];
        Array.Copy(hashBytes, 0, salt, 0, saltSize);
        byte[] hash = new byte[hashSize];
        Array.Copy(hashBytes, saltSize, hash, 0, hashSize);

        // Hash the provided password with the extracted salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        byte[] testHash = pbkdf2.GetBytes(hashSize);

        // Compare the hashes in a way that is safe against timing attacks
        return CryptographicOperations.FixedTimeEquals(testHash, hash);
    }

}
