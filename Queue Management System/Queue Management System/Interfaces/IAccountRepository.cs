using Queue_Management_System.Models;

namespace Queue_Management_System.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<RolesModel>> GetRoles();
        Task<User> GetUserByName(string name);
        Task<bool> CreateUser(User userDetails);
        Task<bool> AddUserToRole(string username, int roleId);
        Task<string> GetPasswordHashAsync(string name);
        Task<List<RolesModel>> GetUserRolesAsync(int userId);
        Task<RolesModel> GetRoleByIdAsync(int RoleId);
        Task<List<ServicePointModel>> GetServicePointsAsync();
        Task<ServicePointModel> GetServicePointAsync(int id);
    }
}
