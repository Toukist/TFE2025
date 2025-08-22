using Dior.Service.Host.Services;
using Dior.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// === CONFIGURATION DES SERVICES ===
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Garde les noms tels quels
    });

// === CONFIGURATION JWT ===
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "TFE2025_JWT_Key_VraimentTresTresLongue_12345!!!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "DiorServiceHost";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "DiorClient";

// Enregistrement des services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Configuration Authentication JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
        
        // Pour debug
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Token authentication failed: {context.Exception}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// === CONFIGURATION CORS - CRITIQUE POUR ANGULAR ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",    // Angular dev
                "http://localhost:5000",    // API HTTP
                "http://localhost:5001",    // API HTTPS (si utilisé)
                "https://localhost:5001",   // API HTTPS
                "https://localhost:7201"    // Port alternatif
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition", "Content-Length");
    });
});

// === CONFIGURATION SWAGGER ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dior API",
        Version = "v1",
        Description = "Backend API pour TFE-2025 - Système de gestion Dior"
    });

    // Configuration JWT dans Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization. Exemple: Bearer {token}"
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
});

// === CONFIGURATION KESTREL (PORTS) ===
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000);                                    // HTTP
    options.ListenLocalhost(5001, config => config.UseHttps());      // HTTPS
});

// === LOGGING ===
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// === PIPELINE MIDDLEWARE ===
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dior API V1");
        c.RoutePrefix = "swagger";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

// Middleware dans l ordre correct
app.UseHttpsRedirection();
app.UseCors("AllowAngular");  // CORS avant Authentication
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// === ENDPOINTS UTILITAIRES ===
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    timestamp = DateTime.Now,
    environment = app.Environment.EnvironmentName 
}));

// === DÉMARRAGE ===
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation(" Dior API démarré sur http://localhost:5000 et https://localhost:5001");
logger.LogInformation(" Swagger disponible sur http://localhost:5000/swagger");

app.Run();
