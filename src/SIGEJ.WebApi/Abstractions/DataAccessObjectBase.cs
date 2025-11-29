using System.Data;
using System.Data.Common;
using Npgsql;
using SIGEJ.WebApi.Data;

namespace SIGEJ.WebApi.Abstractions;

public abstract class DataAccessObjectBase(Database database, ILogger<DataAccessObjectBase> logger) : IDataAccessObject
{
    private async Task<NpgsqlConnection> OpenConnection(CancellationToken cancellationToken = default)
    {
        var conn = database.GetConnection();
        await conn.OpenAsync(cancellationToken);
        return conn;
    }

    protected async Task<IEnumerable<T>> FetchAllAsync<T>(string sql,
        Func<NpgsqlDataReader, CancellationToken, Task<T>> mapper,
        IReadOnlyList<object?>? parameters = null, CancellationToken cancellationToken = default)
    {
        await using var conn = await OpenConnection(cancellationToken);
        await using var cmd = new NpgsqlCommand(sql, conn);
        AddParams(cmd, parameters);
        
        LogSql(cmd.CommandText, parameters);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var list = new List<T>();
        while (await reader.ReadAsync(cancellationToken))
            list.Add(await mapper(reader, cancellationToken));

        return list;
    }

    protected async Task<T?> FetchOneAsync<T>(string sql, Func<NpgsqlDataReader, CancellationToken, Task<T>> mapper,
        IReadOnlyList<object?>? parameters = null, CancellationToken cancellationToken = default)
    {
        await using var conn = await OpenConnection(cancellationToken);
        await using var cmd = new NpgsqlCommand(sql, conn);
        AddParams(cmd, parameters);
        
        LogSql(cmd.CommandText, parameters);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? await mapper(reader, cancellationToken) : default;
    }

    protected async Task ExecuteAsync(string sql, IReadOnlyList<object?>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var conn = await OpenConnection(cancellationToken);
        await using var cmd = new NpgsqlCommand(sql, conn);
        AddParams(cmd, parameters);
        
        LogSql(cmd.CommandText, parameters);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    protected async Task<int> ExecuteReturningIdAsync(string sql, IReadOnlyList<object?>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var conn = await OpenConnection(cancellationToken);
        await using var cmd = new NpgsqlCommand(sql, conn);
        AddParams(cmd, parameters);
        
        LogSql(cmd.CommandText, parameters);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    private static void AddParams(NpgsqlCommand cmd, IReadOnlyList<object?>? parameters)
    {
        if (parameters == null) return;
        for (var i = 0; i < parameters.Count; i++)
            cmd.Parameters.AddWithValue(parameters[i] ?? DBNull.Value);
    }
    
    private void LogSql(string sql, IReadOnlyList<object?>? parameters)
    {
        if (parameters == null || parameters.Count == 0)
        {
            logger.LogInformation("Executando SQL: {Sql}", sql);
        }
        else
        {
            logger.LogInformation("Executando SQL: {Sql} | Parâmetros: {Params}", sql, parameters);
        }
    }
}