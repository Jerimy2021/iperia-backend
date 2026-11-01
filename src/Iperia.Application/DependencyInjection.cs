using Iperia.Application.Ports.In.Auth;
using Iperia.Application.UseCases.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Iperia.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra los casos de uso (puertos de entrada) de la capa de aplicación.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<ILoginUserUseCase, LoginUserUseCase>();

        return services;
    }
}
