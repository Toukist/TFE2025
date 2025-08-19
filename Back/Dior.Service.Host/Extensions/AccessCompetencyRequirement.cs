using Microsoft.AspNetCore.Authorization;

namespace Dior.Service.Host.Extensions
{
    public class AccessCompetencyRequirement : IAuthorizationRequirement
    {
        public string CompetencyCode { get; }

        public AccessCompetencyRequirement(string competencyCode)
        {
            CompetencyCode = competencyCode ?? throw new ArgumentNullException(nameof(competencyCode));
        }
    }
}