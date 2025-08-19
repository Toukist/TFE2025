using Dior.Library.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class UserRoleController : ControllerBase
{
    private readonly string _connectionString;

    public UserRoleController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Dior_DB");
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var list = new List<UserRoleDto>();
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_GetUserRoleList", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new UserRoleDto
                    {
                        Id = (int)reader.GetInt64(reader.GetOrdinal("ID")),
                        RoleDefinitionID = (int)reader.GetInt64(reader.GetOrdinal("RoleDefinitionID")),
                        UserID = (int)reader.GetInt64(reader.GetOrdinal("UserID")),
                        LastEditBy = reader.GetString(reader.GetOrdinal("LastEditBy")),
                        LastEditAt = reader.GetDateTime(reader.GetOrdinal("LastEditAt"))
                    });
                }
            }
        }
        return Ok(list);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(long id)
    {
        UserRoleDto result = null;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_GetUserRoleById", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = new UserRoleDto
                    {
                        Id = (int)reader.GetInt64(reader.GetOrdinal("ID")),
                        RoleDefinitionID = (int)reader.GetInt64(reader.GetOrdinal("RoleDefinitionID")),
                        UserID = (int)reader.GetInt64(reader.GetOrdinal("UserID")),
                        LastEditBy = reader.GetString(reader.GetOrdinal("LastEditBy")),
                        LastEditAt = reader.GetDateTime(reader.GetOrdinal("LastEditAt"))
                    };
                }
            }
        }
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public IActionResult Create([FromBody] UserRoleDto dto)
    {
        long newId;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_AddUserRole", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RoleDefinitionID", dto.RoleDefinitionID); // Correction ici
            cmd.Parameters.AddWithValue("@UserID", dto.UserID);
            cmd.Parameters.AddWithValue("@LastEditBy", dto.LastEditBy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditAt", DateTime.Now);

            conn.Open();
            var result = cmd.ExecuteScalar();
            newId = Convert.ToInt64(result);
        }
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
    }

    [HttpPut("{id}")]
    public IActionResult Update(long id, [FromBody] UserRoleDto dto)
    {
        int rows;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_UpdateUserRole", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@RoleDefinitionID", dto.RoleDefinitionID); // Correction ici
            cmd.Parameters.AddWithValue("@UserID", dto.UserID);
            cmd.Parameters.AddWithValue("@LastEditBy", dto.LastEditBy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditAt", DateTime.Now);

            conn.Open();
            rows = cmd.ExecuteNonQuery();
        }
        if (rows == 0)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(long id)
    {
        int rows;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_DeleteUserRole", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            rows = cmd.ExecuteNonQuery();
        }
        if (rows == 0)
            return NotFound();
        return NoContent();
    }
}
