using Dior.Library.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class PrivilegeController : ControllerBase
{
    private readonly string _connectionString;

    public PrivilegeController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Dior_DB");
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var list = new List<PrivilegeDto>();
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_GetPrivilegeList", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new PrivilegeDto
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                        IsConfigurableRead = reader.GetBoolean(reader.GetOrdinal("isConfigurableRead")),
                        IsConfigurableDelete = reader.GetBoolean(reader.GetOrdinal("isConfigurableDelete")),
                        IsConfigurableAdd = reader.GetBoolean(reader.GetOrdinal("isConfigurableAdd")),
                        IsConfigurableModify = reader.GetBoolean(reader.GetOrdinal("isConfigurableModify")),
                        IsConfigurableStatus = reader.GetBoolean(reader.GetOrdinal("isConfigurableStatus")),
                        IsConfigurableExecution = reader.GetBoolean(reader.GetOrdinal("isConfigurableExecution")),
                        LastEditBy = reader.GetString(reader.GetOrdinal("lastEditBy")),
                        LastEditAt = reader.GetDateTime(reader.GetOrdinal("lastEditAt"))
                    });
                }
            }
        }
        return Ok(list);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(long id)
    {
        PrivilegeDto result = null;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_GetPrivilegeById", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = new PrivilegeDto
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                        LastEditBy = reader.GetString(reader.GetOrdinal("lastEditBy")),
                        LastEditAt = reader.GetDateTime(reader.GetOrdinal("lastEditAt"))
                    };
                }
            }
        }
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public IActionResult Create([FromBody] PrivilegeDto dto)
    {
        long newId;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_AddPrivilege", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", dto.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditBy", dto.LastEditBy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditAt", DateTime.Now);

            conn.Open();
            var result = cmd.ExecuteScalar();
            newId = Convert.ToInt64(result);
        }
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, dto.Name });
    }

    [HttpPut("{id}")]
    public IActionResult Update(long id, [FromBody] PrivilegeDto dto)
    {
        int rows;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_UpdatePrivilege", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", dto.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
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
        using (var cmd = new SqlCommand("sp_DeletePrivilege", conn))
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
