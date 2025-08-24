using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Dior.Service.Host.Extensions;
using Dior.Service.Host.Services;
using Dior.Service.Services;

namespace Dior.Database.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDiorServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<DiorDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DIOR_DB")));

            // Auto-enregistrement des services métier (toutes classes *Service en Scoped)
            var svcAsm = typeof(ContractService).Assembly;
            foreach (var type in svcAsm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service")))
            {
                var iface = type.GetInterfaces().FirstOrDefault();
                if (iface != null)
                    services.AddScoped(iface, type);
                else
                    services.AddScoped(type);
            }

            // Auth JWT
            var jwtSection = configuration.GetSection("Jwt");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var secret = jwtSection["Secret"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "CHANGE_ME"))
                    };
                });

            // Authorization + AccessCompetency policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("competency:PARTYKIT_ACCESS", policy =>
                    policy.Requirements.Add(new AccessCompetencyRequirement("PARTYKIT_ACCESS")));
                options.AddPolicy("competency:USER_CRUD", policy =>
                    policy.Requirements.Add(new AccessCompetencyRequirement("USER_CRUD")));
            });
            services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, AccessCompetencyHandler>();

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("Frontend", b => b
                    .WithOrigins("http://localhost:4200", "https://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });

            // Swagger + Bearer
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dior API", Version = "v1" });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Bearer token only"
                };
                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });
            });
        }
    }
}
