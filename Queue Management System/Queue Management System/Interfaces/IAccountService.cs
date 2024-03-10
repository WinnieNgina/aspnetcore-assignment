namespace Queue_Management_System.Interfaces
{
    public interface IAccountService
    {
        bool VerifyPassword(string password, string storedHash);
        string HashPassword(string password);
    }
}
