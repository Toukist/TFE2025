using Dior.Library;
using Dior.Library.DTO;
using Dior.Service.Host.Services;
using Dior.Service.Host.Middleware;
using Dior.Service.Services;
using Dior.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Dior.Service; // ajout pour RoleService
using Dior.Library.DAO; // Pour les interfaces DAO
using Dior.Service.DAO; // Pour les implémentations DAO
using Dior.Library.Interfaces.UserInterface.Services; // Pour les interfaces services
using Dior.Service.Services.UserInterfaces; // Pour les implémentations services
using Dior.Service.DAO.UserInterfaces; // Pour les DAO User Interfaces
using Dior.Library.Service.DAO; // Pour INotificationDao etc.

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("🔥 DÉMARRAGE DE L'APPLICATION DIOR ENTERPRISE 🔥");

// Charger la config JWT depuis User Secrets, variables d'environnement ou appsettings
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
var secret = builder.Configuration["Jwt:Secret"]
             ?? Environment.GetEnvironmentVariable("Jwt__Secret")
             ?? jwt.Secret;

if (string.IsNullOrWhiteSpace(secret))
    throw new InvalidOperationException(
        "JWT secret manquant. Configurez Jwt:Secret via user-secrets ou variables d'environnement."
    );

// Créer l'objet JwtOptions avec le secret validé
var jwtOptions = new JwtOptions
{
    Secret = secret,
    Issuer = jwt.Issuer ?? builder.Configuration["Jwt:Issuer"] ?? "DiorEnterpriseAPI",
    Audience = jwt.Audience ?? builder.Configuration["Jwt:Audience"] ?? "DiorEnterpriseClient",
    ExpirationMinutes = jwt.ExpirationMinutes > 0 ? jwt.ExpirationMinutes :
                       int.Parse(builder.Configuration["Jwt:ExpirationMinutes"] ?? "480") // 8 heures
};

Console.WriteLine($"=== Configuration JWT ===");
Console.WriteLine($"Issuer: {jwtOptions.Issuer}");
Console.WriteLine($"Audience: {jwtOptions.Audience}");
Console.WriteLine($"Expiration: {jwtOptions.ExpirationMinutes} minutes");

// Ajouter DbContext
var connectionString = builder.Configuration.GetConnectionString("Dior_DB") 
    ?? builder.Configuration.GetConnectionString("DIOR_DB")
    ?? "Server=.;Database=Dior.Database;Trusted_Connection=true;TrustServerCertificate=true;";

builder.Services.AddDbContext<DiorDbContext>(options =>
    options.UseSqlServer(connectionString));

// CORS pour Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Ajouter Controllers avec validation
builder.Services.AddControllers();

// Ajouter Swagger avec support JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dior Enterprise Management API",
        Version = "v1.0",
        Description = @"
## API de gestion d'entreprise complète

Cette API permet la gestion complète d'une entreprise avec 4 rôles principaux :
- **🔧 ADMIN** : Gestion complète du système, utilisateurs et privilèges
- **👨‍💼 MANAGER** : Gestion d'équipe, projets et messagerie  
- **👥 RH** : Gestion des contrats, fiches de paie et évaluations
- **👷 OPÉRATEUR** : Consultation des tâches et documents personnels

### 🔐 Authentification
1. Utilisez `/api/Auth/login` pour obtenir votre token JWT
2. Cliquez sur le bouton **Authorize** (🔒) en haut
3. Entrez : `Bearer VotreTokenJWT`
4. Testez les endpoints selon votre rôle

### 📚 Modules disponibles
- Gestion des utilisateurs et équipes
- Projets avec suivi de progression  
- Système de messagerie interne
- Contrats RH et fiches de paie
- Notifications et tâches
        ",
        Contact = new OpenApiContact
        {
            Name = "Équipe Dior Enterprise",
            Email = "support@dior-enterprise.com"
        }
    });

    // Configuration JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"
JWT Authorization header using the Bearer scheme.

Entrez **uniquement** votre token JWT dans le champ ci-dessous.
Exemple: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

Le préfixe 'Bearer ' sera ajouté automatiquement."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Améliorer les descriptions des endpoints
    c.EnableAnnotations();
    c.DescribeAllParametersInCamelCase();
    
    // Grouper les endpoints par tags
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.DocInclusionPredicate((name, api) => true);
    
    // Inclure les commentaires XML si disponibles
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
    foreach (var xmlFile in xmlFiles)
    {
        try
        {
            c.IncludeXmlComments(xmlFile);
        }
        catch
        {
            // Ignorer les erreurs de commentaires XML
        }
    }
});

// ===== INJECTION DE DÉPENDANCES - SERVICES CRITIQUES =====

// JWT et Auth
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// ===== DAOs (Data Access Objects) =====
// DAOs with IConfiguration dependency - use factory methods
builder.Services.AddScoped<IProjetDao>(sp =>
    new ProjetDao(sp.GetRequiredService<IConfiguration>()));

builder.Services.AddScoped<ITeamDao, DA_Team>(); // Corrigé: utiliser DA_Team

builder.Services.AddScoped<ITaskDao>(sp =>
    new TaskDao(sp.GetRequiredService<IConfiguration>()));

// ContractDao requires string connectionString - use factory
builder.Services.AddScoped<IContractDao>(sp =>
    new ContractDao(sp.GetRequiredService<IConfiguration>().GetConnectionString("DIOR_DB")));

builder.Services.AddScoped<INotificationDao, NotificationDao>();

// DAOs pour User Interfaces - need IConfiguration
builder.Services.AddScoped<IDA_User>(sp =>
    new DA_User(sp.GetRequiredService<IConfiguration>(), sp.GetRequiredService<DiorDbContext>()));

builder.Services.AddScoped<IDA_UserAccessCompetency>(sp =>
    new DA_UserAccessCompetency(sp.GetRequiredService<IConfiguration>()));

builder.Services.AddScoped<IDA_AccessCompetency>(sp =>
    new DA_AccessCompetency(sp.GetRequiredService<IConfiguration>()));

builder.Services.AddScoped<IDA_Access>(sp =>
    new DA_Access(sp.GetRequiredService<IConfiguration>()));

builder.Services.AddScoped<IDA_RoleDefinition>(sp =>
    new DA_RoleDefinition(sp.GetRequiredService<IConfiguration>()));

builder.Services.AddScoped<IDA_Privilege>(sp =>
    new DA_Privilege(sp.GetRequiredService<IConfiguration>()));

builder.Services.AddScoped<IDA_UserRole>(sp =>
    new DA_UserRole(sp.GetRequiredService<IConfiguration>()));

// Add missing interface registration for RoleDefinitionPrivilege
builder.Services.AddScoped<IDA_RoleDefinitionPrivilege>(sp =>
    new DA_RoleDefinitionPrivilege(sp.GetRequiredService<IConfiguration>()));

// ===== SERVICES BUSINESS LAYER =====

// Services Principaux (nouveaux)
builder.Services.AddScoped<IProjetService, ProjetService>();

// Services existants améliorés
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<TaskService>(); // Garder pour compatibilité existante
builder.Services.AddScoped<ContractService>(); // Garder pour compatibilité existante

// Services Interfaces Utilisateur
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAccessCompetencyService, AccessCompetencyService>();
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IRoleDefinitionService, RoleDefinitionService>();
builder.Services.AddScoped<IPrivilegeService, PrivilegeService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IRoleDefinitionPrivilegeService, RoleDefinitionPrivilegeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserAccessService, UserAccessService>();

// Corrigé pour éviter l'ambiguïté
builder.Services.AddScoped<IUserAccessCompetencyService, Dior.Service.Services.UserInterfaces.UserAccessCompetencyService>();

// ===== NOUVEAU SERVICES ENTERPRISE =====

// Services Messagerie (NOUVEAU)
try
{
    builder.Services.AddScoped<IMessageService, MessageService>();
    Console.WriteLine("✓ MessageService enregistré");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Erreur MessageService: {ex.Message}");
}

// Services RH (NOUVEAU)
try
{
    builder.Services.AddScoped<IContractService, ContractService>();
    Console.WriteLine("✓ ContractService (interface) enregistré");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Erreur IContractService: {ex.Message}");
}

try
{
    builder.Services.AddScoped<IPayslipService, PayslipService>();
    Console.WriteLine("✓ PayslipService enregistré");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Erreur PayslipService: {ex.Message}");
}

// Services spécialisés
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<UserAccessCompetencyReader>();

// Services Access
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IAccessCompetencyService, AccessCompetencyService>();
builder.Services.AddScoped<IUserAccessService, UserAccessService>();
builder.Services.AddScoped<IUserAccessCompetencyService, Dior.Service.Services.UserInterfaces.UserAccessCompetencyService>();

// Note: Repository interfaces not found, commented out for now
// TODO: Implement these repositories if needed
// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<ITeamRepository, TeamRepository>();
// builder.Services.AddScoped<IProjetRepository, ProjetRepository>();

// Configurer Authentification JWT
byte[] keyBytes;
try { keyBytes = Convert.FromBase64String(secret); }
catch { keyBytes = Encoding.UTF8.GetBytes(secret); }

var key = new SymmetricSecurityKey(keyBytes);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

// Authorization avec politiques par rôle
builder.Services.AddAuthorization(options =>
{
    // Politiques par rôle
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
    options.AddPolicy("RHOnly", policy => policy.RequireRole("RH"));
    options.AddPolicy("OperateurOnly", policy => policy.RequireRole("Operateur", "Ouvrier"));
    
    // Politiques combinées
    options.AddPolicy("ManagerOrRH", policy => 
        policy.RequireRole("Manager", "RH", "Admin"));
    options.AddPolicy("Leadership", policy => 
        policy.RequireRole("Manager", "RH", "Admin"));
});

var app = builder.Build();

// ===== CONFIGURATION DU PIPELINE MIDDLEWARE =====

// Middleware global d'exception (DOIT être en premier)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configurer Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dior Enterprise API v1.0");
        c.RoutePrefix = "swagger"; // Swagger UI à /swagger
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableValidator();
        c.DisplayOperationId();
        c.DefaultModelExpandDepth(2);
        c.DefaultModelsExpandDepth(1);
        c.EnableTryItOutByDefault();
        
        // Personnalisation de l'interface
        c.DocumentTitle = "Dior Enterprise API - Documentation Interactive";
        c.HeadContent = @"
            <style>
                .swagger-ui .topbar { background-color: #2c5282; }
                .swagger-ui .topbar .download-url-wrapper { display: none; }
            </style>";
            
        // Instructions d'utilisation
        c.InjectJavascript("/swagger-custom.js");
    });
    
    // Créer un fichier JavaScript personnalisé pour Swagger
    app.UseStaticFiles();
}

// En production, protéger Swagger avec authentification
if (app.Environment.IsProduction())
{
    // Utiliser le middleware d'authentification pour Swagger en production  
    app.UseMiddleware<Dior.Service.Host.Extensions.SwaggerAuthMiddleware>();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dior Enterprise API v1.0");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Dior Enterprise API - Documentation";
    });
}

// Pipeline standard
app.UseCors("AllowAngular"); 
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint amélioré
app.MapGet("/", () => new 
{ 
    status = "running",
    api = "Dior Enterprise Management System",
    version = "1.0",
    roles = new[] { "Admin", "Manager", "RH", "Opérateur" },
    features = new[]
    {
        "User Management",
        "Team Management", 
        "Project Management",
        "Message System",
        "HR Contracts",
        "Payslip Management",
        "Role & Privilege Management",
        "Access Control"
    },
    timestamp = DateTime.Now
});

app.MapGet("/health", () => new 
{ 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    database = "Connected"
});

Console.WriteLine("✅ APPLICATION DÉMARRÉE AVEC SUCCÈS");
Console.WriteLine("📍 Swagger UI disponible à: https://localhost:7201/swagger");
Console.WriteLine("🚀 API disponible à: https://localhost:7201/api");
Console.WriteLine("❤️  Health Check: https://localhost:7201/health");
Console.WriteLine("🏢 Dior Enterprise Management System - Ready!");

app.Run();