using Dior.Database.Data;
using Dior.Database.Infrastructure;
using Dior.Database.Services.Implementations;
using Dior.Database.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Dior.Database.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDiorServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration de la base de données
            services.AddDbContext<DiorDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Configuration des services AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

            // Enregistrement des services métiers
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccessService, AccessService>();
            services.AddScoped<IRoleDefinitionService, RoleDefinitionService>();
            services.AddScoped<IPrivilegeService, PrivilegeService>();
            services.AddScoped<IAccessCompetencyService, AccessCompetencyService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IUserAccessCompetencyService, UserAccessCompetencyService>();
            services.AddScoped<IUserAccessService, UserAccessService>();
            services.AddScoped<IRoleDefinitionPrivilegeService, RoleDefinitionPrivilegeService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Dior Database API",
                    Description = "API de gestion des utilisateurs, rôles et compétences pour Dior"
                });
            });
        }
    }
}