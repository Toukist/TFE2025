using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using Dior.Library.Entities;

namespace Dior.Service.Services.UserInterfaces
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IDA_UserRole _DA_UserRole;

        public UserRoleService(IDA_UserRole daUserRole)
        {
            _DA_UserRole = daUserRole;
        }

        public List<UserRole> GetList()
        {
            return _DA_UserRole.GetList();
        }

        public UserRole Get(long id)
        {
            return _DA_UserRole.Get(id);
        }

        public long Add(UserRole entity, string editBy)
        {
            return _DA_UserRole.Add(entity, editBy);
        }

        public void Set(UserRole entity, string editBy)
        {
            _DA_UserRole.Set(entity, editBy);
        }

        public void Del(long id)
        {
            _DA_UserRole.Del(id);
        }
    }
}
