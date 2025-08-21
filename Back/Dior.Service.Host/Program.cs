using Dior.Library.DAO;
using Dior.Library.Entities;
using Dior.Library.Interfaces;
using Dior.Library.Interfaces.DAOs;
using Dior.Library.Interfaces.Services;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using Dior.Service.DAO;
using Dior.Service.DAO.UserInterfaces;
using Dior.Service.DAOs;
using Dior.Service.Host.Extensions;
using Dior.Service.Host.Services; // Pour JwtTokenService et SwaggerAuthMiddleware
using Dior.Service.Services;
using Dior.Service.Services.UserInterfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Injection de Dépendances (Le cœur de la correction de l'erreur 500) ---

// Enregistrement du DbContext
builder.Services.AddDbContext<DiorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DIOR_DB")));

// Enregistrement de tous les DAOs (couche d'accès aux données)
builder.Services.AddScoped<IDA_User, DA_User>();
builder.Services.AddScoped<IDA_Access, DA_Access>();
builder.Services.AddScoped<IDA_RoleDefinition, DA_RoleDefinition>();
builder.Services.AddScoped<IDA_Privilege, DA_Privilege>();
builder.Services.AddScoped<IDA_AccessCompetency, DA_AccessCompetency>();
builder.Services.AddScoped<IDA_UserRole, DA_UserRole>();
builder.Services.AddScoped<IDA_UserAccessCompetency, DA_UserAccessCompetency>();
builder.Services.AddScoped<IDA_RoleDefinitionPrivilege, DA_RoleDefinitionPrivilege>();
builder.Services.AddScoped<ITeamDao, DA_Team>();
builder.Services.AddScoped<ITaskDao, TaskDao>();
builder.Services.AddScoped<IContractDao, ContractDao>();
builder.Services.AddScoped<INotificationDao, NotificationDao>();
// Note : IDA_AuditLog n'a pas d'implémentation directe, il est géré par AuditService

// Enregistrement de tous les Services (couche métier)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IRoleDefinitionService, RoleDefinitionService>();
builder.Services.AddScoped<IPrivilegeService, PrivilegeService>();
builder.Services.AddScoped<IAccessCompetencyService, AccessCompetencyService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IUserAccessService, UserAccessService>();
builder.Services.AddScoped<IUserAccessCompetencyService, Dior.Service.Host.Services.UserAccessCompetencyService>();
builder.Services.AddScoped<IRoleDefinitionPrivilegeService, RoleDefinitionPrivilegeService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<TaskService>(); // Enregistrement de la classe concrète
builder.Services.AddScoped<ContractService>(); // Enregistrement de la classe concrète
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Service pour la génération des tokens JWT
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
// Service pour les rôles
builder.Services.AddScoped<IRoleService, RoleService>();


// --- 2. Configuration de l'application ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// --- 3. Configuration de l'Authentification JWT ---
var jwtKey = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("La clé secrète JWT 'Jwt:Secret' est manquante dans la configuration.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();


// --- 4. Configuration de Swagger ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dior API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez 'Bearer' [espace] puis votre token JWT."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});


// --- Création de l'application ---
var app = builder.Build();


# --- 5. Configuration du Pipeline HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Affiche les erreurs détaillées en développement
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dior API v1"));
}
else
{
    // En production, on peut ajouter une page d'erreur générique et HSTS
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Middleware d'authentification de votre backup pour Swagger
// Assurez-vous que la classe SwaggerAuthMiddleware est bien dans votre projet
app.UseMiddleware<SwaggerAuthMiddleware>();

app.UseHttpsRedirection();

// La séquence est importante : d'abord on authentifie, ensuite on autorise.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();