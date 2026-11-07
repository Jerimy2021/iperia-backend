using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Iperia.Application.Ports.Out;
using Iperia.Infrastructure.Persistence;
using Iperia.Infrastructure.Persistence.Repositories;
using Iperia.Infrastructure.Security;
using Iperia.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Iperia.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra los adaptadores de salida (persistencia, seguridad, etc.).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IperiaDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Falta la cadena de conexión 'IperiaDb'.");
        }

        services.AddDbContext<IperiaDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRevokedTokenStore, RevokedTokenStore>();
        services.AddScoped<IEvidenceRepository, EvidenceRepository>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

        services.AddEvidenceStorage(configuration);

        return services;
    }

    private static IServiceCollection AddEvidenceStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(S3Options.SectionName);
        var s3 = section.Get<S3Options>();
        if (s3 is null || !s3.IsValid())
        {
            throw new InvalidOperationException("Configuración 'S3' inválida: BucketName es obligatorio.");
        }

        services.Configure<S3Options>(section);
        services.AddSingleton<IAmazonS3>(_ => CreateS3Client(s3));
        services.AddSingleton<IEvidenceStorage, S3EvidenceStorage>();

        return services;
    }

    private static AmazonS3Client CreateS3Client(S3Options options)
    {
        var config = new AmazonS3Config
        {
            // Los servicios compatibles con S3 (MinIO) no calculan checksums por defecto como lo hace el SDK nuevo.
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
            ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
        };

        if (!options.UsesCustomEndpoint)
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region);
            // Sin claves en la configuración: el SDK usa el rol IAM o las variables de entorno de AWS.
            return new AmazonS3Client(config);
        }

        config.ServiceURL = options.ServiceUrl;
        config.ForcePathStyle = true;
        config.AuthenticationRegion = options.Region;

        return new AmazonS3Client(new BasicAWSCredentials(options.AccessKey, options.SecretKey), config);
    }
}
