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
                UserName = entity.UserName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Phone = entity.Phone,
                IsActive = entity.IsActive,
                TeamId = entity.TeamId,
                TeamName = entity.Team?.Name
            };
        }

        public static User ToEntity(UserCreateDto dto)
        {
            return new User
            {
                UserName = dto.UserName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                TeamId = dto.TeamId,
                IsActive = true,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System", // TODO: récupérer l'utilisateur courant
                PasswordHash = string.Empty // TODO: hasher le mot de passe
            };
        }

        public static void UpdateEntity(User entity, UserUpdateDto dto)
        {
            entity.UserName = dto.Username;
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.IsActive = dto.IsActive;
            entity.TeamId = dto.TeamId;
            entity.LastEditAt = DateTime.UtcNow;
            entity.LastEditBy = "System"; // TODO: récupérer l'utilisateur courant
        }
    }
}