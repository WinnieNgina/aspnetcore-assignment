namespace Queue_Management_System.Models;
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public int RoleId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}


