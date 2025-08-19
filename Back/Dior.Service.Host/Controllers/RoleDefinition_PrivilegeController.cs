using Dior.Library.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class RoleDefinition_PrivilegeController : ControllerBase
{
    private readonly string _connectionString;

    public RoleDefinition_PrivilegeController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Dior_DB");
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var list = new List<RoleDefinitionPrivilegeDto>();
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_GetRoleDefinitionPrivilegeList", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new RoleDefinitionPrivilegeDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        RoleDefinitionId = reader.GetInt32(reader.GetOrdinal("roleDefinitionId")),
                        PrivilegeId = reader.GetInt32(reader.GetOrdinal("privilegeId")),
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

        RoleDefinitionPrivilegeDto result = null;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_GetRoleDefinitionPrivilegeById", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = new RoleDefinitionPrivilegeDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        RoleDefinitionId = reader.GetInt32(reader.GetOrdinal("roleDefinitionId")),
                        PrivilegeId = reader.GetInt32(reader.GetOrdinal("privilegeId")),
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
    public IActionResult Create([FromBody] RoleDefinitionPrivilegeDto dto)
    {
        int newId;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_AddRoleDefinitionPrivilege", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RoleDefinitionId", dto.RoleDefinitionId);
            cmd.Parameters.AddWithValue("@PrivilegeId", dto.PrivilegeId);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@CreatedBy", dto.CreatedBy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditAt", dto.LastEditAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastEditBy", dto.LastEditBy ?? (object)DBNull.Value);

            conn.Open();
            var result = cmd.ExecuteScalar();
            newId = Convert.ToInt32(result);
        }
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] RoleDefinitionPrivilegeDto dto)
    {
        int rows;
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("sp_UpdateRoleDefinitionPrivilege", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@RoleDefinitionId", dto.RoleDefinitionId);
            cmd.Parameters.AddWithValue("@PrivilegeId", dto.PrivilegeId);
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
        using (var cmd = new SqlCommand("sp_DeleteRoleDefinitionPrivilege", conn))
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
    /// <summary>
    /// Sets the privileges for a specific role by replacing the existing privileges with the provided list.
    /// </summary>
    /// <param name="roleId">The ID of the role to update.</param>
    /// <param name="privilegeIds">The list of privilege IDs to assign to the role.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPost("{roleId}/privileges")]
    public IActionResult SetRolePrivileges(int roleId, [FromBody] List<int> privilegeIds)
    {
        if (privilegeIds == null || privilegeIds.Count == 0)
        {
            return BadRequest("Privilege IDs cannot be null or empty.");
        }

        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    // Delete existing privileges for the role
                    using (var deleteCmd = new SqlCommand("sp_DeleteRolePrivilegesByRoleId", conn, transaction))
                    {
                        deleteCmd.CommandType = CommandType.StoredProcedure;
                        deleteCmd.Parameters.AddWithValue("@RoleId", roleId);
                        deleteCmd.ExecuteNonQuery();
                    }

                    // Insert new privileges for the role
                    foreach (var privilegeId in privilegeIds)
                    {
                        using (var insertCmd = new SqlCommand("sp_AddRolePrivilege", conn, transaction))
                        {
                            insertCmd.CommandType = CommandType.StoredProcedure;
                            insertCmd.Parameters.AddWithValue("@RoleId", roleId);
                            insertCmd.Parameters.AddWithValue("@PrivilegeId", privilegeId);
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    return StatusCode(500, "An error occurred while updating role privileges.");
                }
            }
        }

        return NoContent();
    }

}
