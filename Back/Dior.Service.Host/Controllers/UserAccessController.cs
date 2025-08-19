using Dior.Library.DTO;
using Dior.Library.Interfaces.UserInterface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => /api/UserAccess
    public class UserAccessController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly INotificationService _notificationService;

        public UserAccessController(IConfiguration configuration, INotificationService notificationService)
        {
            _connectionString =
                configuration.GetConnectionString("DIOR_DB")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string introuvable (DIOR_DB ou DefaultConnection).");
            _notificationService = notificationService;
        }

        /// <summary>
        /// PATCH /api/UserAccess/{id}/disable
        /// Désactive un UserAccess (Policy: USER_CRUD)
        /// </summary>
        [HttpPatch("{id:int}/disable")]
        [Authorize(Policy = "USER_CRUD")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Disable(int id, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserId = int.TryParse(userIdClaim, out var uid) ? uid : 1;

            int rows;
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync(ct);
                using var transaction = conn.BeginTransaction();
                
                try
                {
                    // Désactiver l'accès via l'Access lié
                    using var cmd = new SqlCommand(@"
                        UPDATE [ACCESS] 
                        SET IsActive = 0 
                        WHERE Id IN (
                            SELECT AccessId FROM [USER_ACCESS] WHERE Id = @UserAccessId
                        )", conn, transaction);
                    cmd.Parameters.AddWithValue("@UserAccessId", id);
                    rows = await cmd.ExecuteNonQueryAsync(ct);

                    if (rows > 0)
                    {
                        // Créer notification pour admin
                        _notificationService.Add(new NotificationDto
                        {
                            UserId = 1, // Admin
                            Type = "UserAccess",
                            Message = $"UserAccess {id} a été désactivé par l'utilisateur {currentUserId}.",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        });

                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return rows == 0 ? NotFound() : NoContent();
        }

        /// <summary>
        /// PATCH /api/UserAccess/{id}/enable
        /// Active un UserAccess (Policy: USER_CRUD)
        /// </summary>
        [HttpPatch("{id:int}/enable")]
        [Authorize(Policy = "USER_CRUD")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Enable(int id, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserId = int.TryParse(userIdClaim, out var uid) ? uid : 1;

            int rows;
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync(ct);
                using var transaction = conn.BeginTransaction();
                
                try
                {
                    // Activer l'accès via l'Access lié
                    using var cmd = new SqlCommand(@"
                        UPDATE [ACCESS] 
                        SET IsActive = 1 
                        WHERE Id IN (
                            SELECT AccessId FROM [USER_ACCESS] WHERE Id = @UserAccessId
                        )", conn, transaction);
                    cmd.Parameters.AddWithValue("@UserAccessId", id);
                    rows = await cmd.ExecuteNonQueryAsync(ct);

                    if (rows > 0)
                    {
                        // Créer notification pour admin
                        _notificationService.Add(new NotificationDto
                        {
                            UserId = 1, // Admin
                            Type = "UserAccess",
                            Message = $"UserAccess {id} a été activé par l'utilisateur {currentUserId}.",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow
                        });

                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return rows == 0 ? NotFound() : NoContent();
        }

        /// <summary>
        /// GET /api/UserAccess/current
        /// Accès (badge) du user courant (JWT)
        /// </summary>
        [HttpGet("current")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserAccessDto>> Current(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst("userId")?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var dto = await GetUserAccessByUserIdAsync(userId, ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        /// <summary>
        /// GET /api/UserAccess/by-user?userId=123
        /// </summary>
        [HttpGet("by-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByUserId([FromQuery] int userId, CancellationToken ct)
        {
            var result = await GetUserAccessByUserIdAsync(userId, ct);
            if (result is null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// GET /api/UserAccess
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = new List<UserAccessDto>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetUserAccessList", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync(ct);
                using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    list.Add(ReadUserAccess(reader));
                }
            }
            return Ok(list);
        }

        /// <summary>
        /// GET /api/UserAccess/{id}
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            UserAccessDto? result = null;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetUserAccessById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                await conn.OpenAsync(ct);
                using var reader = await cmd.ExecuteReaderAsync(ct);
                if (await reader.ReadAsync(ct))
                {
                    result = ReadUserAccess(reader);
                }
            }
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// POST /api/UserAccess
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] UserAccessDto dto, CancellationToken ct)
        {
            int newId;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_AddUserAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", dto.UserId);
                cmd.Parameters.AddWithValue("@AccessId", dto.AccessId);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)dto.CreatedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LastEditAt", (object?)dto.LastEditAt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LastEditBy", (object?)dto.LastEditBy ?? DBNull.Value);

                await conn.OpenAsync(ct);
                var result = await cmd.ExecuteScalarAsync(ct);
                newId = Convert.ToInt32(result);
            }
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
        }

        /// <summary>
        /// PUT /api/UserAccess/{id}
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UserAccessDto dto, CancellationToken ct)
        {
            int rows;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_UpdateUserAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", dto.UserId);
                cmd.Parameters.AddWithValue("@AccessId", dto.AccessId);
                cmd.Parameters.AddWithValue("@LastEditAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@LastEditBy", (object?)dto.LastEditBy ?? DBNull.Value);

                await conn.OpenAsync(ct);
                rows = await cmd.ExecuteNonQueryAsync(ct);
            }
            return rows == 0 ? NotFound() : NoContent();
        }

        /// <summary>
        /// DELETE /api/UserAccess/{id}
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            int rows;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_DeleteUserAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync(ct);
                rows = await cmd.ExecuteNonQueryAsync(ct);
            }
            return rows == 0 ? NotFound() : NoContent();
        }

        // ----------------- Helpers -----------------

        private static UserAccessDto ReadUserAccess(SqlDataReader reader)
        {
            return new UserAccessDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                AccessId = reader.GetInt32(reader.GetOrdinal("accessId")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("createdBy")) ? null : reader.GetString(reader.GetOrdinal("createdBy")),
                LastEditAt = reader.IsDBNull(reader.GetOrdinal("lastEditAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastEditAt")),
                LastEditBy = reader.IsDBNull(reader.GetOrdinal("lastEditBy")) ? null : reader.GetString(reader.GetOrdinal("lastEditBy"))
            };
        }

        private async Task<UserAccessDto?> GetUserAccessByUserIdAsync(int userId, CancellationToken ct)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetUserAccessList", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync(ct);
            using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                if (reader.GetInt32(reader.GetOrdinal("userId")) == userId)
                    return ReadUserAccess(reader);
            }
            return null;
        }
    }
}
