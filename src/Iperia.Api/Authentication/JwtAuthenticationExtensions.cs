using System.Text;
using Iperia.Application.Ports.Out;
using Iperia.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Iperia.Api.Authentication;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        if (jwt is null || !jwt.IsValid())
        {
            throw new InvalidOperationException(
                "Configuración 'Jwt' inválida: Issuer, Audience y SecretKey (mínimo 32 caracteres) son obligatorios.");
        }

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Mantiene los nombres originales de los claims (sub, email, role...).
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = JwtClaims.Name,
                    RoleClaimType = JwtClaims.Role
                };
                options.Events = new JwtBearerEvents
                {
                    // Rechaza con 401 los tokens cerrados con /auth/logout aunque todavía no hayan expirado.
                    OnTokenValidated = async context =>
                    {
                        var tokenId = context.Principal?.FindFirst(JwtClaims.TokenId)?.Value;
                        var revokedTokens = context.HttpContext.RequestServices.GetRequiredService<IRevokedTokenStore>();

                        if (string.IsNullOrEmpty(tokenId)
                            || await revokedTokens.IsRevokedAsync(tokenId, context.HttpContext.RequestAborted))
                        {
                            context.Fail("El token fue revocado.");
                        }
                    }
                };
            });

        services.AddAuthorization(options => options.AddRolePolicies());

        return services;
    }

    /// <summary>
    /// Agrega el botón "Authorize" en Swagger para enviar el token Bearer.
    /// </summary>
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Pega aquí el accessToken obtenido en /api/auth/login"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
