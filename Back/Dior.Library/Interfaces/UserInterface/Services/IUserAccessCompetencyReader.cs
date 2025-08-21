namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IUserAccessCompetencyReader
    {
        Task<bool> HasCompetencyAsync(int userId, string competencyCode);
        Task<List<string>> GetUserCompetenciesAsync(int userId);
    }
}