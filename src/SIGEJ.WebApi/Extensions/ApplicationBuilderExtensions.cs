using Scalar.AspNetCore;
using SIGEJ.WebApi.Data;

namespace SIGEJ.WebApi;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }

    public static IApplicationBuilder UseScalar(this WebApplication app)
    {
        app.MapScalarApiReference(options =>
        {
            options.Layout = ScalarLayout.Classic;
            options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
            // options.WithTitle("My .NET 8 API with Scalar");
            // options.With("A sample API demonstrating Scalar integration.");
            // You can also customize themes, default HTTP clients, etc.
            // options.Theme = ScalarTheme.Alternate;
        });

        return app;
    }

    public static async Task<IApplicationBuilder> ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        await app.ApplyMigrationsAsync();
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
        await seeder.RunAsync();
        return app;
    }

    public static async Task<IApplicationBuilder> ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<Database>();
        await database.MigrateAsync();
        return app;
    }
}