using Npgsql;

namespace SIGEJ.Api.Data;

public sealed class Database
{
    private readonly string _connectionString;

    public Database(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Database")!;
    }

    public NpgsqlConnection GetConnection() => new(_connectionString);

    public async Task MigrateAsync()
    {
        // TODO: Migrations automation
        // execute every sql script in <cwd>/Migrations
    }
}