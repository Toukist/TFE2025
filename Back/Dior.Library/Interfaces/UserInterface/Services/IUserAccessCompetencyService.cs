using Dior.Library.DTO.Access;

namespace Dior.Library.Interfaces.UserInterface.Services;

public interface IUserAccessCompetencyService
{
<<<<<<< Updated upstream
    // Requ�tes
    List<UserAccessCompetencyDto> GetList();
    UserAccessCompetencyDto? Get(int id);
    List<UserAccessCompetencyDto> GetListByUserId(int userId);

    // Commandes
    int Add(UserAccessCompetencyDto item, string editBy);
    void Set(UserAccessCompetencyDto item, string editBy);
    void Del(int id);

    // M�tier
    bool HasAccessCompetency(int userId, string competencyName);
=======
    public interface IUserAccessCompetencyService
    {
        // Requ�tes
        List<UserAccessCompetencyDto> GetList();
        UserAccessCompetencyDto? Get(long id);
        List<UserAccessCompetencyDto> GetListByUserId(long userId);

        // Commandes
        long Add(UserAccessCompetencyDto item, string editBy);
        void Set(UserAccessCompetencyDto item, string editBy);
        void Del(long id);

        // M�tier
        bool HasAccessCompetency(long userId, string competencyName);
    }
>>>>>>> Stashed changes
}
