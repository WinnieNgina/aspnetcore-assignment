using Npgsql;
using Queue_Management_System.Interfaces;
using Queue_Management_System.Models;
using System.Net;
using System.Xml.Linq;


namespace Queue_Management_System.Repository;

public class AccountRepository :  IAccountRepository
{
    private readonly string connString;

    public AccountRepository(IConfiguration configuration)
    {

        connString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<RolesModel>> GetRoles()
    {
        var roles = new List<RolesModel>();
        string command = "SELECT * FROM roles";
        await using (var connection = new NpgsqlConnection(connString))
        {
            await connection.OpenAsync();

            await using (NpgsqlCommand cmd = new NpgsqlCommand(command, connection))
            {
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var role = new RolesModel
                        {
                            RoleId = (int)reader["id"],
                            RoleName = reader["Name"].ToString(),
                        };
                        roles.Add(role);
                    }
                }
            }
        }
        return roles;
    }
    public async Task<User> GetUserByName(string name)
    {
        var user = new User();
        string command = "SELECT * FROM user WHERE username=$1";
        await using (var connection = new NpgsqlConnection(connString))
        {
            await connection.OpenAsync();
            await using (NpgsqlCommand cmd = new NpgsqlCommand(command, connection))
            {
                cmd.Parameters.AddWithValue("name", name);
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user.Username = reader["username"].ToString();
                        user.Email = reader["email"].ToString();
                        user.PhoneNumber = reader["phonenumber"].ToString();
                        user.Id = (int)reader["id"];
                    }
                }
            }
        }
        return user;
    }
    public async Task<bool> CreateUser(User userDetails)
    {
        string command = "INSERT INTO users (username, passwordhash, email, phonenumber) VALUES($1, $2, $3, $4)";
        await using (var connection = new NpgsqlConnection(connString))
        {
            await connection.OpenAsync();
            await using (var cmd = new NpgsqlCommand(command, connection))
            {
                cmd.Parameters.AddWithValue("username", userDetails.Username);
                cmd.Parameters.AddWithValue("passwordhash", userDetails.Password);
                cmd.Parameters.AddWithValue("email", userDetails.Email);
                cmd.Parameters.AddWithValue("phonenumber", userDetails.PhoneNumber);
                await cmd.ExecuteNonQueryAsync();
            }
        }
        return true;
    }
    public async Task<bool> AddUserToRole(string username, int roleId)
    {
        var user = await GetUserByName(username);
        var command = "INSERT INTO userroles (userid, roleid) VALUES ($1, $2)";
        await using (var connection = new NpgsqlConnection(connString))
        {
            await connection.OpenAsync();
            await using (var cmd = new NpgsqlCommand(command, connection))
            {
                cmd.Parameters.AddWithValue("userid", user.Id);
                cmd.Parameters.AddWithValue("roleid", roleId);
                await cmd.ExecuteNonQueryAsync();
            }
            return true;
        }
    }
    public async Task<string> GetPasswordHashAsync(string name)
    {
        string command = "SELECT  passwordhash FROM user WHERE username=$1";
        await using (var connection = new NpgsqlConnection(connString))
        {
            await connection.OpenAsync();
            await using (NpgsqlCommand cmd = new NpgsqlCommand(command, connection))
            {
                cmd.Parameters.AddWithValue("name", name);
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader.GetString(0);
                    }
                }
            }
        }
        return null;
    }
    public async Task<List<RolesModel>> GetUserRolesAsync(int userId)
    {
        var roles = new List<RolesModel>();
        string command = "SELECT * FROM userroles WHERE id=$1";
        await using (var connection = new NpgsqlConnection(connString))
        {
            await connection.OpenAsync();
            await using (var cmd = new NpgsqlCommand(command, connection))
            {
                cmd.Parameters.AddWithValue("id", userId);
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var role = await GetRoleByIdAsync((int)reader["id"]);
                        roles.Add(role);
                    }
                }
            }
        }
        return roles;
    }
    public async Task<RolesModel> GetRoleByIdAsync(int RoleId)
    {
        var role = new RolesModel();
        string command = "SELECT * FROM roles WHERE id=$1";
        await using (var connection = new NpgsqlConnection(connString))
        {
            await connection.OpenAsync();
            await using (var cmd = new NpgsqlCommand(command, connection))
            {
                cmd.Parameters.AddWithValue("id", RoleId);
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        role.RoleId = (int)reader["id"];
                        role.RoleName = reader["Name"].ToString();
                    }
                }
            }
        }
        return role;
    }
    public async Task<List<ServicePointModel>> GetServicePointsAsync()
    {
        var servicePoints = new List<ServicePointModel>();
        string command = "SELECT * FROM servicepoints";
        await using (var connection = new NpgsqlConnection(connString))
        {
            await connection.OpenAsync();

            await using (NpgsqlCommand cmd = new NpgsqlCommand(command, connection))
            {
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var servicepoint = new ServicePointModel
                        {
                            Id = (int)reader["id"],
                            Name = reader["Name"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["createdat"]),
                            CreatedBy = reader["createdby"].ToString(),
                            Description = reader["description"].ToString(),
                            Location = reader["location"].ToString()
                        };
                        servicePoints.Add(servicepoint);
                    }
                }
            }
        }
        return servicePoints;
    }
    public async Task<ServicePointModel> GetServicePointAsync(int id)
    {
        var servicePoint = new ServicePointModel();
        string command = "SELECT * FROM servicepoints WHERE id=$1";
        await using (var connection = new NpgsqlConnection(connString))
        {
            await connection.OpenAsync();
            await using (var cmd = new NpgsqlCommand(command, connection))
            {
                cmd.Parameters.AddWithValue("id", id);
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        servicePoint.Id = (int)reader["id"];
                        servicePoint.Name = reader["Name"].ToString();
                        servicePoint.CreatedAt = Convert.ToDateTime(reader["createdat"]);
                        servicePoint.CreatedBy = reader["createdby"].ToString();
                        servicePoint.Description = reader["description"].ToString();
                        servicePoint.Location = reader["location"].ToString();
                    }
                }
            }
        }
        return servicePoint;
    }
}
