using Dior.Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dior.Database.Data
{
    public class DiorDbContext : DbContext
    {
        public DiorDbContext(DbContextOptions<DiorDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<RoleDefinition> RoleDefinitions { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<AccessCompetency> AccessCompetencies { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserAccessCompetency> UserAccessCompetencies { get; set; }
        public DbSet<UserAccess> UserAccesses { get; set; }
        public DbSet<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Uniques et Index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<Access>()
                .HasIndex(a => a.BadgePhysicalNumber)
                .IsUnique();

            modelBuilder.Entity<RoleDefinition>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Privilege>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<AccessCompetency>()
                .HasIndex(a => a.Name)
                .IsUnique();

            // Relations de clé composite unique
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleDefinitionId })
                .IsUnique();

            modelBuilder.Entity<UserAccessCompetency>()
                .HasIndex(uac => new { uac.UserId, uac.AccessCompetencyId })
                .IsUnique();

            modelBuilder.Entity<UserAccess>()
                .HasIndex(ua => new { ua.UserId, ua.AccessId })
                .IsUnique();

            modelBuilder.Entity<RoleDefinitionPrivilege>()
                .HasIndex(rp => new { rp.RoleDefinitionId, rp.PrivilegeId })
                .IsUnique();

            // Relations auto-référentielles
            modelBuilder.Entity<RoleDefinition>()
                .HasMany(r => r.ChildRoles)
                .WithOne(r => r.ParentRole)
                .HasForeignKey(r => r.ParentRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccessCompetency>()
                .HasMany(a => a.Children)
                .WithOne(a => a.Parent)
                .HasForeignKey(a => a.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration des suppressions en cascade
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.RoleDefinition)
                .WithMany(rd => rd.UserRoles)
                .HasForeignKey(ur => ur.RoleDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserAccessCompetency>()
                .HasOne(uac => uac.User)
                .WithMany(u => u.UserAccessCompetencies)
                .HasForeignKey(uac => uac.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAccessCompetency>()
                .HasOne(uac => uac.AccessCompetency)
                .WithMany(ac => ac.UserAccessCompetencies)
                .HasForeignKey(uac => uac.AccessCompetencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserAccess>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAccesses)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAccess>()
                .HasOne(ua => ua.Access)
                .WithMany(a => a.UserAccesses)
                .HasForeignKey(ua => ua.AccessId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleDefinitionPrivilege>()
                .HasOne(rdp => rdp.RoleDefinition)
                .WithMany(rd => rd.RoleDefinitionPrivileges)
                .HasForeignKey(rdp => rdp.RoleDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoleDefinitionPrivilege>()
                .HasOne(rdp => rdp.Privilege)
                .WithMany(p => p.RoleDefinitionPrivileges)
                .HasForeignKey(rdp => rdp.PrivilegeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}