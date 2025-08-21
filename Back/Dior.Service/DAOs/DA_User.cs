using Dior.Library.Interfaces.DAOs;
using Dior.Library.Entities;
using Dior.Service.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAOs
{
    public class DA_User : IDA_User
    {
        private readonly string _connectionString;
        private readonly DiorDbContext _context;

        public DA_User(IConfiguration configuration, DiorDbContext context)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            
            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey) 
                ?? throw new InvalidOperationException("Database connection string not found");
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public User GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetUserByUsername", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapUser(reader);
                }
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging instead of empty catch
                // _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw; // Re-throw for now, implement proper error handling later
            }
            return null;
        }

        public User GetUserById(long id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapUser(reader);
                }
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging
                throw;
            }
            return null;
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = new List<User>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetAllUsers", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(MapUser(reader));
                }
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging
                throw;
            }
            return users;
        }

        public void CreateUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_AddUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Username", user.Username ?? string.Empty);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? string.Empty);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName ?? string.Empty);
                cmd.Parameters.AddWithValue("@LastName", user.LastName ?? string.Empty);
                cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TeamId", user.TeamId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy ?? string.Empty);
                
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging
                throw;
            }
        }

        public void UpdateUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_UpdateUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", user.Id);
                cmd.Parameters.AddWithValue("@Username", user.Username ?? string.Empty);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName ?? string.Empty);
                cmd.Parameters.AddWithValue("@LastName", user.LastName ?? string.Empty);
                cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TeamId", user.TeamId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@LastEditBy", user.LastEditBy ?? string.Empty);
                
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging
                throw;
            }
        }

        public void DeleteUser(long id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_DeleteUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging
                throw;
            }
        }

        private static User MapUser(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Username = reader["Username"]?.ToString() ?? string.Empty,
                PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty,
                FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString(),
                Phone = reader["Phone"]?.ToString(),
                TeamId = reader.IsDBNull(reader.GetOrdinal("TeamId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TeamId")),
                IsActive = !reader.IsDBNull(reader.GetOrdinal("IsActive")) && reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader["CreatedBy"]?.ToString() ?? string.Empty,
                LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                LastEditBy = reader["LastEditBy"]?.ToString()
            };
        }
    }
}