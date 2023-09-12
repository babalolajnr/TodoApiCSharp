namespace TodoApi.Auth;

/// <summary>
/// Helper methods for authentication.
/// </summary>
class Helpers
{
    /// <summary>
    /// Hashes a password using BCrypt.Net.
    /// </summary>
    /// <param name="password">String to be hashed.</param>
    /// <returns>A hashed representation of <paramref name="password"/>.</returns>
    public static string HashPassword(string password)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt();

        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    /// <summary>
    /// Verifies a password against a hash using BCrypt.Net.
    /// </summary>
    /// <param name="inputPassword"></param>
    /// <param name="hashedPassword"></param>
    /// <returns></returns>
    public static bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
    }

}