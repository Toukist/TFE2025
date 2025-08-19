using Dior.Library;
using Dior.Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Services
{
    public class DiorDbContext : DbContext, IDiorContext
    {
        public DiorDbContext(DbContextOptions<DiorDbContext> options) : base(options) { }

        // DbSets pour les entités principales (conformes à IDiorContext)
        public DbSet<User> User { get; set; }
        public DbSet<Access> Access { get; set; }
        public DbSet<RoleDefinition> RoleDefinitions { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<AccessCompetency> AccessCompetencies { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        
        // Tables de liaison (non exposées dans l'API)
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleDefinitionPrivilege> RoleDefinitionPrivileges { get; set; }
        public DbSet<UserAccessCompetency> UserAccessCompetencies { get; set; }
        public DbSet<UserAccess> UserAccesses { get; set; }
        public object Accesses { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");
                entity.HasKey(e => e.Id);
                
                // Index unique sur Email
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_User_Email");
                
                // Index unique sur UserName
                entity.HasIndex(e => e.UserName)
                    .IsUnique()
                    .HasDatabaseName("IX_User_UserName");

                // Relations
                entity.HasOne(e => e.Team)
                    .WithMany()
                    .HasForeignKey(e => e.TeamId)
                    .IsRequired(false);

                entity.HasMany(e => e.UserRoles)
                    .WithOne(ur => ur.User)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.UserAccesses)
                    .WithOne(ua => ua.User)
                    .HasForeignKey(ua => ua.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.UserAccessCompetencies)
                    .WithOne(uac => uac.User)
                    .HasForeignKey(uac => uac.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.AuditLogs)
                    .WithOne(al => al.User)
                    .HasForeignKey(al => al.UserId)
                    .IsRequired(false);
            });

            // Configuration Access
            modelBuilder.Entity<Access>(entity =>
            {
                entity.ToTable("ACCESS");
                entity.HasKey(e => e.Id);
                
                // Index unique sur BadgePhysicalNumber
                entity.HasIndex(e => e.BadgePhysicalNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Access_BadgePhysicalNumber")
                    .HasFilter("[BadgePhysicalNumber] IS NOT NULL");

                entity.HasMany(e => e.UserAccesses)
                    .WithOne(ua => ua.Access)
                    .HasForeignKey(ua => ua.AccessId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration RoleDefinition
            modelBuilder.Entity<RoleDefinition>(entity =>
            {
                entity.ToTable("ROLE_DEFINITION");
                entity.HasKey(e => e.Id);

                entity.HasMany(e => e.UserRoles)
                    .WithOne(ur => ur.RoleDefinition)
                    .HasForeignKey(ur => ur.RoleDefinitionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.RoleDefinitionPrivileges)
                    .WithOne(rdp => rdp.RoleDefinition)
                    .HasForeignKey(rdp => rdp.RoleDefinitionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration Privilege
            modelBuilder.Entity<Privilege>(entity =>
            {
                entity.ToTable("PRIVILEGE");
                entity.HasKey(e => e.Id);

                entity.HasMany(e => e.RoleDefinitionPrivileges)
                    .WithOne(rdp => rdp.Privilege)
                    .HasForeignKey(rdp => rdp.PrivilegeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration AccessCompetency
            modelBuilder.Entity<AccessCompetency>(entity =>
            {
                entity.ToTable("ACCESS_COMPETENCY");
                entity.HasKey(e => e.Id);

                entity.HasMany(e => e.UserAccessCompetencies)
                    .WithOne(uac => uac.AccessCompetency)
                    .HasForeignKey(uac => uac.AccessCompetencyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration Team
            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("TEAM");
                entity.HasKey(e => e.Id);
            });

            // Configuration Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("NOTIFICATION");
                entity.HasKey(e => e.Id);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AUDIT_LOG");
                entity.HasKey(e => e.Id);
            });

            // Configuration des tables de liaison (clés composites)
            
            // UserRole - table de liaison User <-> RoleDefinition
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("USER_ROLE");
                entity.HasKey(e => new { e.UserId, e.RoleDefinitionId });
            });

            // RoleDefinitionPrivilege - table de liaison RoleDefinition <-> Privilege
            modelBuilder.Entity<RoleDefinitionPrivilege>(entity =>
            {
                entity.ToTable("ROLE_DEFINITION_PRIVILEGE");
                entity.HasKey(e => new { e.RoleDefinitionId, e.PrivilegeId });
            });

            // UserAccessCompetency - table de liaison User <-> AccessCompetency
            modelBuilder.Entity<UserAccessCompetency>(entity =>
            {
                entity.ToTable("USER_ACCESS_COMPETENCY");
                entity.HasKey(e => new { e.UserId, e.AccessCompetencyId });
            });

            // UserAccess - table de liaison User <-> Access
            modelBuilder.Entity<UserAccess>(entity =>
            {
                entity.ToTable("USER_ACCESS");
                entity.HasKey(e => e.Id);
                
                // Index pour éviter les doublons User/Access
                entity.HasIndex(e => new { e.UserId, e.AccessId })
                    .IsUnique()
                    .HasDatabaseName("IX_UserAccess_User_Access");
            });
        }
    }
}