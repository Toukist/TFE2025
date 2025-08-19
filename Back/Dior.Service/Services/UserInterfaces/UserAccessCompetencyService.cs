using Dior.Library.Entities;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;

namespace Dior.Service.Services.UserInterfaces
{
    public class UserAccessCompetencyService : IUserAccessCompetencyService
    {
        private readonly IDA_UserAccessCompetency _DA_UserAccessCompetency;

        public UserAccessCompetencyService(IDA_UserAccessCompetency DA_UserAccessCompetency)
        {
            _DA_UserAccessCompetency = DA_UserAccessCompetency;
        }

        public List<UserAccessCompetencyDto> GetList()
        {
            var list = _DA_UserAccessCompetency.GetList();
            return list.ConvertAll(x => new UserAccessCompetencyDto
            {
                Id = x.Id,
                UserId = x.UserId,
                AccessCompetencyId = x.AccessCompetencyId,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                LastEditAt = x.LastEditAt,
                LastEditBy = x.LastEditBy
            });
        }

        public List<UserAccessCompetencyDto> GetListByUserId(int userId)
        {
            return _DA_UserAccessCompetency.GetList()
                .Where(x => x.UserId == userId)
                .Select(x => new UserAccessCompetencyDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    AccessCompetencyId = x.AccessCompetencyId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    LastEditAt = x.LastEditAt,
                    LastEditBy = x.LastEditBy
                })
                .ToList();
        }

        public UserAccessCompetencyDto? Get(int id)
        {
            var x = _DA_UserAccessCompetency.Get(id);
            return x == null ? null : new UserAccessCompetencyDto
            {
                Id = x.Id,
                UserId = x.UserId,
                AccessCompetencyId = x.AccessCompetencyId,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                LastEditAt = x.LastEditAt,
                LastEditBy = x.LastEditBy
            };
        }

        public int Add(UserAccessCompetencyDto dto, string editBy)
        {
            var entity = new UserAccessCompetency
            {
                Id = dto.Id,
                UserId = dto.UserId,
                AccessCompetencyId = dto.AccessCompetencyId,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastEditAt = dto.LastEditAt,
                LastEditBy = dto.LastEditBy
            };
            return (int)_DA_UserAccessCompetency.Add(entity, editBy);
        }

        public void Set(UserAccessCompetencyDto dto, string editBy)
        {
            var entity = new UserAccessCompetency
            {
                Id = dto.Id,
                UserId = dto.UserId,
                AccessCompetencyId = dto.AccessCompetencyId,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastEditAt = dto.LastEditAt,
                LastEditBy = dto.LastEditBy
            };
            _DA_UserAccessCompetency.Set(entity, editBy);
        }

        public void Del(int id)
        {
            _DA_UserAccessCompetency.Del(id);
        }

        public bool HasAccessCompetency(int userId, string competencyName)
        {
            return _DA_UserAccessCompetency.HasAccessCompetency(userId, competencyName);
        }
    }
}
