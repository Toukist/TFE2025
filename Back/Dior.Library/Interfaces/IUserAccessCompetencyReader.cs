namespace Dior.Library.Interfaces
{
    public interface IUserAccessCompetencyReader
    {
        Task<bool> HasCompetencyAsync(int userId, string competencyCode);
        Task<List<string>> GetUserCompetenciesAsync(int userId);
    }
}