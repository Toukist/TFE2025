using Microsoft.EntityFrameworkCore;
using Dior.Library.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Dior.Library.Interfaces
{
    public interface IDiorContext
    {
        DbSet<User> User { get; }
        DbSet<Access> Access { get; }
        DbSet<RoleDefinition> RoleDefinitions { get; }
        DbSet<Privilege> Privileges { get; }
        DbSet<AccessCompetency> AccessCompetencies { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; }
        DbSet<UserAccessCompetency> UserAccessCompetencies { get; }
        DbSet<UserAccess> UserAccesses { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<Team> Teams { get; }
        DbSet<Notification> Notification { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}