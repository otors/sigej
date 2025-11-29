using Npgsql;

namespace SIGEJ.WebApi.Data;

public sealed class Database
{
    private readonly string _connectionString;

    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    public NpgsqlConnection GetConnection() => new(_connectionString);

    public async Task MigrateAsync()
    {
        // TODO: Migrations automation
        // execute every sql script in <cwd>/Migrations
    }
}