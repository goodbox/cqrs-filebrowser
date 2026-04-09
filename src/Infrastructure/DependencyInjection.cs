using Application.Common;
using Infrastructure.FileSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new FileSystemSettings();
        configuration.GetSection("FileSystem").Bind(settings);

        services.AddSingleton<IFileSystemSettings>(settings);
        services.AddScoped<IFileSystemRepository, PhysicalFileSystemRepository>();

        return services;
    }
}
