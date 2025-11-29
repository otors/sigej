using System.Reflection;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;

namespace SIGEJ.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
            typeof(DataAccessObjectBase).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract).ToList();

        foreach (var type in types)
            services.AddSingleton(type);

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Database")!;
        services.AddSingleton(sp => new Database(cs, sp.GetRequiredService<ILogger<Database>>()));
        services.AddSingleton<Seeder>();
        return services;
    }
}