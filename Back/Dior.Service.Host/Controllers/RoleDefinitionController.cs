using Dior.Library.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class RoleDefinitionController : ControllerBase
{
    private readonly string _connectionString;

    public RoleDefinitionController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Dior_DB");
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var list = new List<RoleDefinitionDto>();
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_GetRoleDefinitionList", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new RoleDefinitionDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                        ParentRoleId = reader.IsDBNull(reader.GetOrdinal("parentRoleId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("parentRoleId")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("isActive")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdAt")),
                        CreatedBy = reader.IsDBNull(reader.GetOrdinal("createdBy")) ? null : reader.GetString(reader.GetOrdinal("createdBy")),
                        LastEditAt = reader.IsDBNull(reader.GetOrdinal("lastEditAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastEditAt")),
                        LastEditBy = reader.IsDBNull(reader.GetOrdinal("lastEditBy")) ? null : reader.GetString(reader.GetOrdinal("lastEditBy"))
                    });
                }
            }
        }
        return Ok(list);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        RoleDefinitionDto result = null;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_GetRoleDefinitionById", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = new RoleDefinitionDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                        ParentRoleId = reader.IsDBNull(reader.GetOrdinal("parentRoleId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("parentRoleId")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("isActive")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdAt")),
                        CreatedBy = reader.IsDBNull(reader.GetOrdinal("createdBy")) ? null : reader.GetString(reader.GetOrdinal("createdBy")),
                        LastEditAt = reader.IsDBNull(reader.GetOrdinal("lastEditAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastEditAt")),
                        LastEditBy = reader.IsDBNull(reader.GetOrdinal("lastEditBy")) ? null : reader.GetString(reader.GetOrdinal("lastEditBy"))
                    };
                }
            }
        }
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public IActionResult Create([FromBody] RoleDefinitionDto dto)
    {
        int newId;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_AddRoleDefinition", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", dto.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
            //cmd.Parameters.AddWithValue("@ParentRoleId", dto.ParentRoleId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ParentId", DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@CreatedBy", dto.CreatedBy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditAt", dto.LastEditAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditBy", dto.LastEditBy ?? (object)DBNull.Value);

            conn.Open();
            var result = cmd.ExecuteScalar();
            newId = Convert.ToInt32(result);
        }
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, dto.Name });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] RoleDefinitionDto dto)
    {
        int rows;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_UpdateRoleDefinition", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", dto.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ParentRoleId", dto.ParentRoleId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
            cmd.Parameters.AddWithValue("@LastEditAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@LastEditBy", dto.LastEditBy ?? (object)DBNull.Value);

            conn.Open();
            rows = cmd.ExecuteNonQuery();
        }
        if (rows == 0)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        int rows;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_DeleteRoleDefinition", conn))
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
