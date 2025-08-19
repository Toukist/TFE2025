using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;

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
            // Convert types if needed
            return _DA_UserRole.GetList();
        }

        public UserRole Get(long id)
        {
            // Implement method to get user role by id
            return _DA_UserRole.Get(id);
        }

        public long Add(UserRole userRole, string editBy)
        {
            return _DA_UserRole.Add(userRole, editBy);
        }

        public void Set(UserRole userRole, string editBy)
        {
            _DA_UserRole.Set(userRole, editBy);
        }

        public void Del(long id)
        {
            _DA_UserRole.Del(id);
        }
    }
}
