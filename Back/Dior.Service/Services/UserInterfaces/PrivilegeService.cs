using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using Dior.Library.Entities;

namespace Dior.Service.Services.UserInterfaces
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IDA_Privilege _daPrivilege;

        public PrivilegeService(IDA_Privilege daPrivilege)
        {
            _daPrivilege = daPrivilege;
        }

        public Privilege Get(long id)
        {
            var boPrivilege = _daPrivilege.Get(id);
            return MapToEntityPrivilege(boPrivilege);
        }

        public List<Privilege> GetList()
        {
            var boList = _daPrivilege.GetList();
            return boList.Select(MapToEntityPrivilege).ToList();
        }

        public long Add(Privilege privilege, string editBy)
        {
            return _daPrivilege.Add(privilege, editBy);
        }

        public void Set(Privilege privilege, string editBy)
        {
            _daPrivilege.Set(privilege, editBy);
        }

        public void Del(long id)
        {
            _daPrivilege.Del(id);
        }

        private Privilege MapToEntityPrivilege(Dior.Library.BO.UserInterface.Privilege boPrivilege)
        {
            if (boPrivilege == null) return null;
            return new Privilege
            {
                Id = boPrivilege.Id,
                Name = boPrivilege.Name,
                Description = boPrivilege.Description,
                IsActive = boPrivilege.IsActive
            };
        }
    }
}
