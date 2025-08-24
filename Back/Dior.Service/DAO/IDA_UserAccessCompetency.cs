using Dior.Library.Entities;

namespace Dior.Library.Service.DAO
{
    public interface IDA_UserAccessCompetency
    {
        List<UserAccessCompetency> GetList();
        UserAccessCompetency Get(int id);
        int Add(UserAccessCompetency entity, string editBy);
        void Set(UserAccessCompetency entity, string editBy);
        void Del(int id);
        bool HasAccessCompetency(int userId, string competencyName);
    }
}
