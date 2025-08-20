using Dior.Library.DTO;
using Dior.Library.Entities;

namespace Dior.Service.Services.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(User entity)
        {
            return new UserDto
            {
                Id = entity.Id,
                Username = entity.Username, // Correction: utiliser Username
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Phone = entity.Phone,
                IsActive = entity.IsActive,
                TeamId = entity.TeamId,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                LastEditAt = entity.LastEditAt,
                LastEditBy = entity.LastEditBy
            };
        }

        public static User ToEntity(UserDto dto)
        {
            return new User
            {
                Id = dto.Id,
                Username = dto.Username, // Correction: utiliser Username
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                IsActive = dto.IsActive,
                TeamId = dto.TeamId,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastEditAt = dto.LastEditAt,
                LastEditBy = dto.LastEditBy
            };
        }

        public static void UpdateEntity(User entity, UserDto dto)
        {
            entity.Username = dto.Username; // Correction: utiliser Username
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.IsActive = dto.IsActive;
            entity.TeamId = dto.TeamId;
            entity.LastEditAt = DateTime.UtcNow;
            entity.LastEditBy = dto.LastEditBy;
        }
    }
}