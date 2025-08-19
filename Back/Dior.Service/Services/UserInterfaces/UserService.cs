using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using DiorUser = Dior.Library.BO.UserInterface.User;

namespace Dior.Service.Services.UserInterfaces
{
    public class UserService : IUserService
    {
        private readonly IDA_User _DA_User;

        public UserService(IDA_User daUser)
        {
            _DA_User = daUser;
        }

        public List<DiorUser> GetList(List<int> userIDs = null)
        {
            var users = _DA_User.GetList(userIDs);
            return users != null ? users.ConvertAll(user => new DiorUser
            {
                Id = user.Id,
                Name = user.Name,
                // Map other properties as needed
            }) : new List<DiorUser>();
        }

        public void Set(DiorUser user, string editBy)
        {
            _DA_User.Set(new User
            {
                Id = user.Id,
                Name = user.Name,
                // Map other properties as needed
            }, editBy);
        }

        public int Add(DiorUser user, string editBy)
        {
            return _DA_User.Add(new User
            {
                Id = user.Id,
                Name = user.Name,
                // Map other properties as needed
            }, editBy);
        }

        public void Del(int id)
        {
            _DA_User.Del(id);
        }

        public DiorUser Get(int userId)
        {
            throw new NotImplementedException();
        }

        public object Add(User user, string editBy)
        {
            throw new NotImplementedException();
        }

        public void Set(User user, string editBy)
        {
            throw new NotImplementedException();
        }

        public List<DiorUser> GetList(List<long> userIDs = null)
        {
            throw new NotImplementedException();
        }

        long IUserService.Add(DiorUser user, string editBy)
        {
            return Add(user, editBy);
        }

        public void Del(long id)
        {
            throw new NotImplementedException();
        }

        public DiorUser Get(long userId)
        {
            throw new NotImplementedException();
        }
    }
}
