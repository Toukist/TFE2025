using Dior.Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dior.Library
{
    public interface IDiorContext
    {
        DbSet<User> User { get; set; }
        DbSet<Access> Access { get; set; }
        DbSet<RoleDefinition> RoleDefinitions { get; set; }
        DbSet<Privilege> Privileges { get; set; }
        DbSet<AccessCompetency> AccessCompetencies { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; set; }
        DbSet<UserAccessCompetency> UserAccessCompetencies { get; set; }
        DbSet<UserAccess> UserAccesses { get; set; }
        DbSet<AuditLog> AuditLogs { get; set; }
        DbSet<Team> Teams { get; set; }
        DbSet<Notification> Notifications { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
