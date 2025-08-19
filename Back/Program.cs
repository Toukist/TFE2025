using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using Dior.Service.DAO.UserInterfaces;
using Dior.Service.Host.Services;
using Dior.Service.Services.UserInterfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Text;

var corsPolicyName = "AllowAngularDev";
var builder = WebApplication.CreateBuilder(args);

// 1) DbContext
builder.Services.AddDbContext<DiorDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DIOR_DB")));

// 2) Services métiers & DAO
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDA_User, DA_User>(); // Replace `DA_User` with the actual implementation class
// … tes autres AddScoped/AddSingleton …

// 3) JWT Token Service
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// 4) Authentification JWT
var secretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey manquant dans la config");
var issuer = builder.Configuration["Jwt:Issuer"] ?? "DiorServiceHost";
var audience = builder.Configuration["Jwt:Audience"] ?? "DiorClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opts =>
  {
      opts.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = issuer,
          ValidAudience = audience,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
          ClockSkew = TimeSpan.Zero
      };
  });

// 5) Controllers & Authorize
builder.Services.AddControllers();
builder.Services.AddAuthorization();

// 6) CORS pour Angular
builder.Services.AddCors(o =>
{
    o.AddPolicy(corsPolicyName, p =>
      p.WithOrigins("http://localhost:4200")
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowCredentials());
});

// 7) Swagger / JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Dior API",
        Description = "Gestion utilisateurs, rôles et accès"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id   = "Bearer"
        }
      },
      Array.Empty<string>()
    }
  });
});

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dior API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    // protèger accès
    app.UseMiddleware<SwaggerAuthMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dior API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// **CORS avant tout le reste d’accès à l’API**
app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
