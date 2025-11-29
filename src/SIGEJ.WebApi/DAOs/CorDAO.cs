using System.Data;
using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class CorDAO(Database database, ILogger<CorDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(Cor cor, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO cor (nome) VALUES ($1) RETURNING id",
            [cor.Nome],
            cancellationToken
        );
    }

    public async Task<Cor?> FindByIdAsync(int corId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, nome FROM cor WHERE id = $1",
            MapCorAsync,
            [corId],
            cancellationToken
        );
    }

    public async Task<List<Cor>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, nome FROM cor ORDER BY nome",
            MapCorAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<Cor> MapCorAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new Cor
        {
            Id = r.GetInt32(0),
            Nome = await r.GetNullableFieldValueAsync<string>(1, cancellationToken),
        };
    }

}