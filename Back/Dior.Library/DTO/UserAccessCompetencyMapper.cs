using Dior.Library.Entities;

namespace Dior.Library.DTO
{
    public static class UserAccessCompetencyMapper
    {
        public static UserAccessCompetencyDto ToDto(this UserAccessCompetency entity)
        {
            if (entity == null) return null;
            return new UserAccessCompetencyDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                AccessCompetencyId = entity.AccessCompetencyId,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                LastEditAt = entity.LastEditAt,
                LastEditBy = entity.LastEditBy
            };
        }

        public static UserAccessCompetency ToEntity(this UserAccessCompetencyDto dto)
        {
            if (dto == null) return null;
            return new UserAccessCompetency
            {
                Id = dto.Id,
                UserId = dto.UserId,
                AccessCompetencyId = dto.AccessCompetencyId,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastEditAt = dto.LastEditAt,
                LastEditBy = dto.LastEditBy
            };
        }
    }
}
