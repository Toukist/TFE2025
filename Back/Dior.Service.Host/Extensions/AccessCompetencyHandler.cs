using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Dior.Library.Interfaces.UserInterface.Services;

namespace Dior.Service.Host.Extensions
{
    public class AccessCompetencyHandler : AuthorizationHandler<AccessCompetencyRequirement>
    {
        private readonly IUserAccessCompetencyReader _competencyReader;

        public AccessCompetencyHandler(IUserAccessCompetencyReader competencyReader)
        {
            _competencyReader = competencyReader;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AccessCompetencyRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst("userId")?.Value
                           ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? context.User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                context.Fail();
                return;
            }

            var hasCompetency = await _competencyReader.HasCompetencyAsync(userId, requirement.CompetencyCode);

            if (hasCompetency)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}