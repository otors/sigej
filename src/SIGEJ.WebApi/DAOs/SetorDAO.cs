using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class SetorDAO(Database database, ILogger<SetorDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(Setor setor, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO setor (nome, sigla) VALUES ($1, $2) RETURNING id",
            [setor.Nome, setor.Sigla], cancellationToken
        );
    }

    public async Task<Setor?> FindByIdAsync(int setorId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync("SELECT id, nome, sigla FROM setor WHERE id = $1", MapSetorAsync, [setorId],
            cancellationToken);
    }

    public async Task<List<Setor>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync("SELECT id, nome, sigla FROM setor ORDER BY nome", MapSetorAsync,
            cancellationToken: cancellationToken)).ToList();
    }

    private static async Task<Setor> MapSetorAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new Setor
        {
            Id = r.GetInt32(0),
            Nome = r.GetString(1),
            Sigla = await r.GetNullableFieldValueAsync<string>(2, cancellationToken)
        };
    }
}