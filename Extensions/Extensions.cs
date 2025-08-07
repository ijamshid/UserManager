using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using UserManager.Application.Options;

namespace UserManager.Presentation.Extensions;

public static class Extensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Audience = "cafe.uz";

                var signInKey = configuration["Jwt:Key"];

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudiences = [ "UserManager.uz"],
                    ValidIssuers = ["UserManager.uz"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signInKey))
                };
            });

        return services;
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
            .BindConfiguration("Jwt");

        return services;
    }
   
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            // Bir nechta variantni tekshiramiz
            var idValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? user.FindFirst("sub")?.Value
                        ?? user.FindFirst("id")?.Value;

            if (int.TryParse(idValue, out var id))
                return id;
            return null;
        }
    
    public static IServiceCollection AddSessions(this IServiceCollection services)
    {
       services.AddDistributedMemoryCache(); // Session uchun in-memory cache
       services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Session vaqti
            options.Cookie.HttpOnly = true; // Xavfsizlik uchun
            options.Cookie.IsEssential = true; // GDPR uchun
        });


        return services;
    }
}