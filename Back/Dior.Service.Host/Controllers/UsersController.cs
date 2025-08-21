using System.Data;
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dior.Library.DTOs;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    public class UsersController : ControllerBase
    {
        private readonly string _cs;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IConfiguration config, ILogger<UsersController> logger)
        {
            _cs = config.GetConnectionString("DefaultConnection")
                  ?? config.GetConnectionString("DIOR_DB")
                  ?? throw new InvalidOperationException("Connection string manquante (DefaultConnection ou DIOR_DB)");
            _logger = logger;
        }

        // GET /api/users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            try
            {
                var list = await GetUsersWithRolesInternal();
                return Ok(list);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Erreur SQL dans GetAll");
                return Problem("Erreur lors de la récupération des utilisateurs.");
            }
        }

        // GET /api/users/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            try
            {
                UserDto? dto = null;
                await using (var conn = CreateConnection())
                await using (var cmd = new SqlCommand("sp_GetUserById", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        dto = MapUser(reader);
                    }
                }
                if (dto == null) return NotFound();

                dto.Roles = await GetUserRoleDefinitionsAsync(id);
                return Ok(dto);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Erreur SQL dans GetById");
                return Problem("Erreur lors de la récupération de l'utilisateur.");
            }
        }

        /// <summary>
        /// Obtenir les infos complètes d'un utilisateur avec équipe et manager
        /// </summary>
        [HttpGet("{id}/full")]
        public async Task<ActionResult<UserDto>> GetFullInfo(long id)
        {
            try
            {
                var user = await GetUserWithRolesAsync((int)id);
                if (user == null) return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetFullInfo");
                return Problem("Erreur lors de la récupération des informations complètes.");
            }
        }

        /// <summary>
        /// Obtenir les membres de mon équipe (Manager)
        /// </summary>
        [HttpGet("my-team")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<List<UserDto>>> GetMyTeamMembers()
        {
            try
            {
                var managerId = GetCurrentUserId();
                var members = new List<UserDto>();
                
                return Ok(members);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans GetMyTeamMembers");
                return Problem("Erreur lors de la récupération des membres de l'équipe.");
            }
        }

        // POST /api/users
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto dto)
        {
            if (dto == null) return BadRequest("Payload manquant");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                int newId;
                string? passwordHash = null;
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }

                await using var conn = CreateConnection();
                await conn.OpenAsync();
                await using var tx = await conn.BeginTransactionAsync();
                try
                {
                    await using (var cmd = new SqlCommand("sp_AddUser", conn, (SqlTransaction)tx)
                    { CommandType = CommandType.StoredProcedure })
                    {
                        AddParam(cmd, "@IsActive", dto.IsActive, SqlDbType.Bit); 
                        AddParam(cmd, "@Username", dto.UserName, SqlDbType.NVarChar, 100);
                        AddParam(cmd, "@LastName", dto.LastName, SqlDbType.NVarChar, 100);
                        AddParam(cmd, "@FirstName", dto.FirstName, SqlDbType.NVarChar, 100);
                        AddParam(cmd, "@Email", dto.Email, SqlDbType.NVarChar, 255);
                        AddParam(cmd, "@Phone", (object?)dto.Phone ?? DBNull.Value, SqlDbType.NVarChar, 50);
                        AddParam(cmd, "@PasswordHash", (object?)passwordHash ?? DBNull.Value, SqlDbType.NVarChar, 200);
                        var scalar = await cmd.ExecuteScalarAsync();
                        newId = Convert.ToInt32(scalar);
                    }

                    if (dto.RoleIds?.Count > 0)
                    {
                        foreach (var roleId in dto.RoleIds.Distinct())
                        {
                            await using var cmdRole = new SqlCommand("sp_AddUserRole", conn, (SqlTransaction)tx)
                            { CommandType = CommandType.StoredProcedure };
                            AddParam(cmdRole, "@UserId", newId, SqlDbType.Int);
                            AddParam(cmdRole, "@RoleDefinitionId", roleId, SqlDbType.Int);
                            await cmdRole.ExecuteNonQueryAsync();
                        }
                    }

                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    throw new Exception("Echec transaction création utilisateur", ex);
                }

                var created = await GetUserWithRolesAsync(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, created);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Erreur SQL dans Create");
                return Problem("Erreur lors de la création de l'utilisateur.");
            }
        }

        // PUT /api/users/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            if (dto == null) return BadRequest("Payload manquant");
            if (id != dto.Id) return BadRequest("Id incohérent");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var existing = await GetUserWithRolesAsync(id);
                if (existing == null) return NotFound();

                string? passwordHash = null;
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }

                await using var conn = CreateConnection();
                await conn.OpenAsync();
                await using var tx = await conn.BeginTransactionAsync();
                try
                {
                    await using (var cmd = new SqlCommand("sp_UpdateUser", conn, (SqlTransaction)tx) { CommandType = CommandType.StoredProcedure })
                    {
                        AddParam(cmd, "@Id", id, SqlDbType.Int);
                        AddParam(cmd, "@IsActive", dto.IsActive, SqlDbType.Bit);
                        AddParam(cmd, "@Username", dto.Username, SqlDbType.NVarChar, 100);
                        AddParam(cmd, "@LastName", dto.LastName, SqlDbType.NVarChar, 100);
                        AddParam(cmd, "@FirstName", dto.FirstName, SqlDbType.NVarChar, 100);
                        AddParam(cmd, "@Email", dto.Email, SqlDbType.NVarChar, 255);
                        AddParam(cmd, "@Phone", (object?)dto.Phone ?? DBNull.Value, SqlDbType.NVarChar, 50);
                        AddParam(cmd, "@PasswordHash", (object?)passwordHash ?? DBNull.Value, SqlDbType.NVarChar, 200);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    var currentRoleMap = await GetUserRoleIdMapAsync(id);
                    var newRoleIds = dto.RoleIds?.Distinct().ToList() ?? new List<int>();

                    foreach (var addId in newRoleIds.Where(r => !currentRoleMap.ContainsKey(r)))
                    {
                        await using var addCmd = new SqlCommand("sp_AddUserRole", conn, (SqlTransaction)tx)
                        { CommandType = CommandType.StoredProcedure };
                        AddParam(addCmd, "@UserId", id, SqlDbType.Int);
                        AddParam(addCmd, "@RoleDefinitionId", addId, SqlDbType.Int);
                        await addCmd.ExecuteNonQueryAsync();
                    }

                    var toRemove = currentRoleMap.Keys.Where(k => !newRoleIds.Contains(k)).ToList();
                    foreach (var rid in toRemove)
                    {
                        var linkId = currentRoleMap[rid];
                        await using var delCmd = new SqlCommand("sp_DeleteUserRole", conn, (SqlTransaction)tx)
                        { CommandType = CommandType.StoredProcedure };
                        AddParam(delCmd, "@Id", linkId, SqlDbType.Int);
                        await delCmd.ExecuteNonQueryAsync();
                    }

                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    throw new Exception("Echec transaction mise à jour utilisateur", ex);
                }

                return NoContent();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Erreur SQL dans Update");
                return Problem("Erreur lors de la mise à jour de l'utilisateur.");
            }
        }

        // DELETE /api/users/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await using var conn = CreateConnection();
                await using var cmd = new SqlCommand("sp_DeleteUser", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return NoContent();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Erreur SQL dans Delete");
                return Problem("Erreur lors de la suppression de l'utilisateur.");
            }
        }

        // ===================== Helpers =====================
        private SqlConnection CreateConnection() => new SqlConnection(_cs);

        private static void AddParam(SqlCommand cmd, string name, object? value, SqlDbType type, int? size = null)
        {
            var p = cmd.Parameters.Add(name, type);
            if (size.HasValue) p.Size = size.Value;
            p.Value = value ?? DBNull.Value;
        }

        private static UserDto MapUser(SqlDataReader r)
        {
            return new UserDto
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                Username = SafeString(r, "Username") ?? string.Empty,
                FirstName = SafeString(r, "FirstName") ?? string.Empty,
                LastName = SafeString(r, "LastName") ?? string.Empty,
                Email = SafeString(r, "Email"),
                Phone = SafeString(r, "Phone"),
                TeamName = SafeString(r, "TeamName"),
                IsActive = SafeBool(r, "IsActive"),
                CreatedAt = SafeDateTime(r, "CreatedAt"),
                CreatedBy = SafeString(r, "CreatedBy") ?? string.Empty,
                LastEditAt = SafeDateTimeNullable(r, "LastEditAt"),
                LastEditBy = SafeString(r, "LastEditBy"),
                Roles = new List<RoleDefinitionDto>()
            };
        }

        private async Task<UserDto?> GetUserWithRolesAsync(int id)
        {
            UserDto? user = null;
            await using (var conn = CreateConnection())
            await using (var cmd = new SqlCommand("sp_GetUserById", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Id", id);
                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    user = MapUser(reader);
                }
            }
            if (user != null)
            {
                user.Roles = await GetUserRoleDefinitionsAsync(id);
            }
            return user;
        }

        private async Task<List<RoleDefinitionDto>> GetUserRoleDefinitionsAsync(int userId)
        {
            var roles = new List<RoleDefinitionDto>();
            await using var conn = CreateConnection();
            await using var cmd = new SqlCommand("sp_GetUserRoleList", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", userId);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var role = new RoleDefinitionDto
                {
                    Id = SafeInt(reader, "RoleId") ?? 0,
                    Name = SafeString(reader, "RoleName") ?? string.Empty
                };
                if (role.Id > 0 && !roles.Any(r => r.Id == role.Id))
                {
                    roles.Add(role);
                }
            }
            return roles;
        }

        private async Task<Dictionary<int, int>> GetUserRoleIdMapAsync(int userId)
        {
            var map = new Dictionary<int, int>();
            await using var conn = CreateConnection();
            await using var cmd = new SqlCommand("sp_GetUserRoleList", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", userId);
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int roleId = SafeInt(reader, "RoleId") ?? 0;
                int linkId = SafeInt(reader, "Id") ?? 0;
                if (roleId > 0 && !map.ContainsKey(roleId)) map.Add(roleId, linkId);
            }
            return map;
        }

        private async Task<List<UserDto>> GetUsersWithRolesInternal()
        {
            var dict = new Dictionary<int, UserDto>();
            await using var conn = CreateConnection();
            await using var cmd = new SqlCommand("sp_GetUsersWithRoles", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var userId = SafeInt(reader, "UserId") ?? 0;
                if (userId == 0) continue;
                if (!dict.TryGetValue(userId, out var u))
                {
                    u = new UserDto
                    {
                        Id = userId,
                        Username = SafeString(reader, "Username") ?? string.Empty,
                        FirstName = SafeString(reader, "FirstName") ?? string.Empty,
                        LastName = SafeString(reader, "LastName") ?? string.Empty,
                        Email = SafeString(reader, "Email"),
                        Phone = SafeString(reader, "Phone"),
                        TeamName = SafeString(reader, "TeamName"),
                        IsActive = SafeBool(reader, "IsActive"),
                        CreatedAt = SafeDateTime(reader, "CreatedAt"),
                        CreatedBy = SafeString(reader, "CreatedBy") ?? string.Empty,
                        LastEditAt = SafeDateTimeNullable(reader, "LastEditAt"),
                        LastEditBy = SafeString(reader, "LastEditBy"),
                        Roles = new List<RoleDefinitionDto>()
                    };
                    dict.Add(userId, u);
                }
                var roleId = SafeInt(reader, "RoleId") ?? 0;
                var roleName = SafeString(reader, "RoleName");
                if (roleId > 0 && !string.IsNullOrWhiteSpace(roleName) && !u.Roles.Any(r => r.Id == roleId))
                {
                    u.Roles.Add(new RoleDefinitionDto { Id = roleId, Name = roleName });
                }
            }
            return dict.Values.ToList();
        }

        private long GetCurrentUserId()
        {
            return long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        // ===================== Safe reader helpers =====================
        private static string? SafeString(SqlDataReader r, string col)
        {
            int idx; try { idx = r.GetOrdinal(col); } catch { return null; }
            return r.IsDBNull(idx) ? null : r.GetString(idx);
        }
        private static bool SafeBool(SqlDataReader r, string col)
        {
            int idx; try { idx = r.GetOrdinal(col); } catch { return false; }
            return !r.IsDBNull(idx) && r.GetBoolean(idx);
        }
        private static int? SafeInt(SqlDataReader r, string col)
        {
            int idx; try { idx = r.GetOrdinal(col); } catch { return null; }
            return r.IsDBNull(idx) ? (int?)null : r.GetInt32(idx);
        }
        private static DateTime SafeDateTime(SqlDataReader r, string col)
        {
            int idx; try { idx = r.GetOrdinal(col); } catch { return DateTime.MinValue; }
            return r.IsDBNull(idx) ? DateTime.MinValue : r.GetDateTime(idx);
        }
        private static DateTime? SafeDateTimeNullable(SqlDataReader r, string col)
        {
            int idx; try { idx = r.GetOrdinal(col); } catch { return null; }
            return r.IsDBNull(idx) ? (DateTime?)null : r.GetDateTime(idx);
        }
    }
}