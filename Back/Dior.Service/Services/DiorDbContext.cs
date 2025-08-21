using Dior.Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Services;

public class DiorDbContext : DbContext
{
    public DiorDbContext(DbContextOptions<DiorDbContext> options) : base(options) { }

    // DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Access> Accesses { get; set; } = null!;
    public DbSet<RoleDefinition> RoleDefinitions { get; set; } = null!;
    public DbSet<Privilege> Privileges { get; set; } = null!;
    public DbSet<AccessCompetency> AccessCompetencies { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; set; } = null!;
    public DbSet<UserAccessCompetency> UserAccessCompetencies { get; set; } = null!;
    public DbSet<UserAccess> UserAccesses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasIndex(e => e.BadgePhysicalNumber).IsUnique()
                .HasFilter("[BadgePhysicalNumber] IS NOT NULL");
        });

        // Configuration Access
        modelBuilder.Entity<Access>(entity =>
        {
            entity.HasIndex(e => e.BadgePhysicalNumber).IsUnique()
                .HasFilter("[BadgePhysicalNumber] IS NOT NULL");
        });

        // Configuration des clés composites
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleDefinitionId });

        modelBuilder.Entity<RoleDefinitionPrivilege>()
            .HasKey(rdp => new { rdp.RoleDefinitionId, rdp.PrivilegeId });

        modelBuilder.Entity<UserAccessCompetency>()
            .HasKey(uac => new { uac.UserId, uac.AccessCompetencyId });

        modelBuilder.Entity<UserAccess>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.AccessId }).IsUnique();
        });
    }
}