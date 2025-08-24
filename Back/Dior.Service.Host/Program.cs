using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dior.Service.Host.Services;

var builder = WebApplication.CreateBuilder(args);

// ======================== JSON CONFIG ========================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ======================== DB CONTEXT ========================
builder.Services.AddDbContext<DiorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DIOR_DB")));

// ======================== SERVICES MÉTIER ========================
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<ProjetService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ContractService>();
builder.Services.AddScoped<AccessService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<UserRoleService>();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// ======================== LOGGING ========================
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// ======================== JWT CONFIG ========================
var jwtSecret = builder.Configuration["Jwt:Secret"];
SymmetricSecurityKey signingKey;

try
{
    // Si c’est une chaîne Base64 valide → on décode
    var keyBytes = Convert.FromBase64String(jwtSecret);
    signingKey = new SymmetricSecurityKey(keyBytes);
    Console.WriteLine("✅ JWT Secret décodé en Base64.");
}
catch
{
    // Sinon → on considère que c’est une clé texte brut
    signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret ?? "DiorSuperSecretKeyForDevOnly!"));
    Console.WriteLine("⚠️ JWT Secret utilisé en clair (pas Base64).");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

// ======================== CORS ========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ======================== SWAGGER ========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dior API",
        Version = "v1",
        Description = "API Backend pour l'application Dior"
    });

    // Support JWT dans Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT sous la forme: Bearer {token}"
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

    // Inclure les commentaires XML si présents
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// ======================== PIPELINE ========================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dior API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("DevelopmentPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ======================== TEST ENDPOINT ========================
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
