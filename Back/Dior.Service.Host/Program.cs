using Dior.Database.Extensions;
using Dior.Service.Host.Extensions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Configuration JSON avec camelCase
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// DbContext
builder.Services.AddDbContext<DiorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DIOR_DB")));

// Services métier
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

// Logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// JWT Configuration
var jwtSecret = builder.Configuration["Jwt:Secret"] 
    ?? "DiorSuperSecretKeyForJWTTokenGeneration2024!"; // Clé par défaut pour développement

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

// CORS
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

// Swagger
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
        Description = "Entrez votre token JWT"
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

    // Inclure les commentaires XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Test endpoint
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
