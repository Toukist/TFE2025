using Dior.Library;
using Dior.Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Services
{
    public class DiorDbContext : DbContext, IDiorContext
    {
        public DiorDbContext() { }
        
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("TempDb");
            }
        }

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
                entity.HasIndex(e => e.Username)
                    .IsUnique()
                    .HasDatabaseName("IX_User_UserName");
            });

            // Configuration Access
            modelBuilder.Entity<Access>(entity =>
            {
                entity.ToTable("ACCESS");
                entity.HasKey(e => e.Id);
            });

            // Configuration RoleDefinition
            modelBuilder.Entity<RoleDefinition>(entity =>
            {
                entity.ToTable("ROLE_DEFINITION");
                entity.HasKey(e => e.Id);
            });

            // Configuration Privilege
            modelBuilder.Entity<Privilege>(entity =>
            {
                entity.ToTable("PRIVILEGE");
                entity.HasKey(e => e.Id);
            });

            // Configuration AccessCompetency
            modelBuilder.Entity<AccessCompetency>(entity =>
            {
                entity.ToTable("ACCESS_COMPETENCY");
                entity.HasKey(e => e.Id);
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
                entity.HasKey(e => e.Id);
            });

            // UserAccess - table de liaison User <-> Access
            modelBuilder.Entity<UserAccess>(entity =>
            {
                entity.ToTable("USER_ACCESS");
                entity.HasKey(e => e.Id);
            });
        }
    }
}