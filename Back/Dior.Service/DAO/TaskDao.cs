using Dior.Library.BO;
using Dior.Library.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dior.Service.DAO
{
    public class TaskDao : ITaskDao
    {
        private readonly string _connectionString;

        public TaskDao(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"] ?? "DIOR_DB";
            _connectionString = configuration.GetConnectionString(activeDbKey)
                ?? throw new InvalidOperationException($"Connection string '{activeDbKey}' not found");
        }

        public List<TaskBO> GetAll()
        {
            var list = new List<TaskBO>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM TASK", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapFromReader(reader));
                    }
                }
            }
            return list;
        }

        public TaskBO? GetById(long id)
        {
            TaskBO? result = null;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM TASK WHERE ID = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = MapFromReader(reader);
                    }
                }
            }
            return result;
        }

        public void Create(TaskBO task)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
                INSERT INTO TASK (Title, Description, Status, AssignedToUserId, CreatedByUserId, 
                                CreatedAt, CreatedBy, LastEditAt, LastEditBy)
                VALUES (@Title, @Description, @Status, @AssignedToUserId, @CreatedByUserId,
                        @CreatedAt, @CreatedBy, @LastEditAt, @LastEditBy);
                SELECT SCOPE_IDENTITY();", conn))
            {
                MapToParameters(cmd, task);
                conn.Open();
                var result = cmd.ExecuteScalar();
                task.Id = Convert.ToInt32(result);
            }
        }

        public void Update(TaskBO task)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
                UPDATE TASK SET 
                    Title = @Title,
                    Description = @Description,
                    Status = @Status,
                    AssignedToUserId = @AssignedToUserId,
                    CreatedByUserId = @CreatedByUserId,
                    CreatedAt = @CreatedAt,
                    CreatedBy = @CreatedBy,
                    LastEditAt = @LastEditAt,
                    LastEditBy = @LastEditBy
                WHERE ID = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", task.Id);
                MapToParameters(cmd, task);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(long id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("DELETE FROM TASK WHERE ID = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private TaskBO MapFromReader(SqlDataReader reader)
        {
            return new TaskBO
            {
                Id = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("ID"))),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = !reader.IsDBNull(reader.GetOrdinal("Description")) ? reader.GetString(reader.GetOrdinal("Description")) : null,
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AssignedToUserId = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("AssignedToUserId"))),
                CreatedByUserId = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("CreatedByUserId"))),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                LastEditAt = !reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? reader.GetDateTime(reader.GetOrdinal("LastEditAt")) : null,
                LastEditBy = !reader.IsDBNull(reader.GetOrdinal("LastEditBy")) ? reader.GetString(reader.GetOrdinal("LastEditBy")) : null
            };
        }

        private void MapToParameters(SqlCommand cmd, TaskBO task)
        {
            cmd.Parameters.AddWithValue("@Title", task.Title);
            cmd.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", task.Status);
            cmd.Parameters.AddWithValue("@AssignedToUserId", task.AssignedToUserId);
            cmd.Parameters.AddWithValue("@CreatedByUserId", task.CreatedByUserId);
            cmd.Parameters.AddWithValue("@CreatedAt", task.CreatedAt);
            cmd.Parameters.AddWithValue("@CreatedBy", task.CreatedBy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditAt", task.LastEditAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditBy", task.LastEditBy ?? (object)DBNull.Value);
        }

        public List<TaskBO> GetTasksAssignedToUser(long userId)
        {
            var list = new List<TaskBO>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
        SELECT * FROM TASK 
        WHERE AssignedToUserId = @UserId 
        ORDER BY CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapFromReader(reader));
                    }
                }
            }
            return list;
        }

        public List<TaskBO> GetTasksCreatedByUser(long userId)
        {
            var list = new List<TaskBO>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
        SELECT * FROM TASK 
        WHERE CreatedByUserId = @UserId 
        ORDER BY CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapFromReader(reader));
                    }
                }
            }
            return list;
        }

        public List<TaskBO> GetTasksByStatus(string status)
        {
            var list = new List<TaskBO>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
        SELECT * FROM TASK 
        WHERE Status = @Status 
        ORDER BY CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@Status", status);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapFromReader(reader));
                    }
                }
            }
            return list;
        }

        public int GetTaskCountByStatusForUser(long userId, string status)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
        SELECT COUNT(*) FROM TASK 
        WHERE AssignedToUserId = @UserId AND Status = @Status", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Status", status);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void UpdateTaskStatus(long taskId, string newStatus, string lastEditBy)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
        UPDATE TASK SET 
            Status = @Status,
            LastEditAt = @LastEditAt,
            LastEditBy = @LastEditBy
        WHERE ID = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", taskId);
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@LastEditAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@LastEditBy", lastEditBy);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}