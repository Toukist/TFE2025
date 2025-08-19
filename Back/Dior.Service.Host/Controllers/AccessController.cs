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
    [Route("api/[controller]")]
    public class AccessController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IAccessService _accessService;
        private readonly INotificationService _notificationService;

        public AccessController(IConfiguration configuration, IAccessService accessService, INotificationService notificationService)
        {
            _connectionString = configuration.GetConnectionString("Dior_DB");
            _accessService = accessService;
            _notificationService = notificationService;
        }

        // --- PATCH admin/rh ---
        /// <summary>
        /// PATCH api/Access/{id}/disable
        /// Désactive un badge (admin/rh)
        /// </summary>
        [HttpPatch("{id:int}/disable")]
        [Authorize(Roles = "admin,rh")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Disable(int id, CancellationToken ct)
        {
            var ok = await _accessService.SetActiveAsync(id, false, ct);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>
        /// PATCH api/Access/{id}/enable
        /// Active un badge (admin/rh)
        /// </summary>
        [HttpPatch("{id:int}/enable")]
        [Authorize(Roles = "admin,rh")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Enable(int id, CancellationToken ct)
        {
            var ok = await _accessService.SetActiveAsync(id, true, ct);
            return ok ? NoContent() : NotFound();
        }

        // --- PATCH self (routes existantes) ---
        [Authorize]
        [HttpPatch("self-disable")]
        public IActionResult DisableOwnBadge()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            int? badgeId = GetAccessIdForUser(userId);
            if (badgeId == null)
                return NotFound("Aucun badge lié à cet utilisateur.");

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("UPDATE Access SET isActive = 0 WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", badgeId.Value);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                    return NotFound();
            }

            _notificationService.Add(new NotificationDto
            {
                UserId = 1, // Admin
                Type = "Badge",
                Message = $"L'utilisateur {userId} a désactivé son badge.",
                IsRead = false,
                CreatedAt = DateTime.Now
            });

            return Ok("Badge désactivé et notification envoyée.");
        }

        [Authorize]
        [HttpPatch("self-enable")]
        public IActionResult EnableOwnBadge()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            int? badgeId = GetAccessIdForUser(userId);
            if (badgeId == null)
                return NotFound("Aucun badge lié à cet utilisateur.");

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("UPDATE Access SET isActive = 1 WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", badgeId.Value);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                    return NotFound();
            }

            _notificationService.Add(new NotificationDto
            {
                UserId = 1, // Admin
                Type = "Badge",
                Message = $"L'utilisateur {userId} a réactivé son badge.",
                IsRead = false,
                CreatedAt = DateTime.Now
            });

            return Ok("Badge réactivé et notification envoyée.");
        }

        // --- CRUD Standard ---
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _accessService.GetList();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            AccessDto result = null;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetAccessById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = new AccessDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            BadgePhysicalNumber = reader.GetString(reader.GetOrdinal("badgePhysicalNumber")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("isActive")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdAt")),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("createdBy")) ? null : reader.GetString(reader.GetOrdinal("createdBy"))
                        };
                    }
                }
            }

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AccessDto dto)
        {
            int newId;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_AddAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BadgePhysicalNumber", dto.BadgePhysicalNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreatedBy", dto.CreatedBy ?? (object)DBNull.Value);

                conn.Open();
                var result = cmd.ExecuteScalar();
                newId = Convert.ToInt32(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, dto.BadgePhysicalNumber });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AccessDto dto)
        {
            int rows;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_UpdateAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@BadgePhysicalNumber", dto.BadgePhysicalNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", dto.CreatedBy ?? (object)DBNull.Value);

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
            using (var cmd = new SqlCommand("sp_DeleteAccess", conn))
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

        // 🔧 Méthode utilitaire pour retrouver le badge d’un utilisateur
        private int? GetAccessIdForUser(int userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT TOP 1 AccessId FROM UserAccess WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : (int?)null;
            }
        }
    }
}
