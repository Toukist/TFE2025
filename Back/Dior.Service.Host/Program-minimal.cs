using Microsoft.AspNetCore;

namespace Dior.Service.Host
{
    public class ProgramMinimal
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Services de base uniquement
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.MapControllers();
            app.MapGet("/", () => "API Dior - Test Minimal");
            app.MapGet("/health", () => new { status = "OK", timestamp = DateTime.Now });
            
            Console.WriteLine("🔥 API Dior Test Minimal démarrée sur http://localhost:5100");
            
            app.Run();
        }
    }
}
