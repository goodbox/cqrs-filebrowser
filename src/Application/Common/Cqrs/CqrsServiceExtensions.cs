using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Cqrs;

public static class CqrsServiceExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        var assembly = typeof(CqrsServiceExtensions).Assembly;

        foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface))
        {
            foreach (var iface in type.GetInterfaces().Where(i => i.IsGenericType))
            {
                var def = iface.GetGenericTypeDefinition();
                if (def == typeof(IRequestHandler<,>) || def == typeof(IRequestHandler<>))
                    services.AddTransient(iface, type);
            }
        }

        services.AddTransient<ISender, Mediator>();
        return services;
    }
}
