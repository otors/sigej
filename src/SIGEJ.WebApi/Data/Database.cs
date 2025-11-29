using Npgsql;

namespace SIGEJ.WebApi.Data;

public sealed class Database
{
    private readonly string _connectionString;
    private readonly ILogger<Database> _logger;

    public Database(string connectionString, ILogger<Database> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public NpgsqlConnection GetConnection() => new(_connectionString);

    public async Task MigrateAsync()
    {
        var migrationsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Migrations");

        if (!Directory.Exists(migrationsFolder))
            throw new DirectoryNotFoundException($"Pasta de migrations não encontrada: {migrationsFolder}");

        // Pega todos os arquivos .sql, ordenados por nome
        var sqlFiles = Directory.GetFiles(migrationsFolder, "*.sql").OrderBy(f => f);

        await using var conn = GetConnection();
        await conn.OpenAsync();

        foreach (var file in sqlFiles)
        {
            var sql = await File.ReadAllTextAsync(file);

            await using var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
            
            _logger.LogInformation("Migrated {File}", Path.GetFileName(file));
        }
    }
}