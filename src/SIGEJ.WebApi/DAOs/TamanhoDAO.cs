using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class TamanhoDAO(Database database, ILogger<TamanhoDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(Tamanho tamanho, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO tamanho (descricao) VALUES ($1) RETURNING id",
            [tamanho.Descricao],
            cancellationToken
        );
    }

    public async Task<Tamanho?> FindByIdAsync(int tamanhoId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, descricao FROM tamanho WHERE id = $1",
            MapTamanhoAsync,
            [tamanhoId],
            cancellationToken
        );
    }

    public async Task<List<Tamanho>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, descricao FROM tamanho ORDER BY descricao",
            MapTamanhoAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }
    
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync("DELETE FROM tamanho WHERE id = $1", [id], cancellationToken);
    }

    private static async Task<Tamanho> MapTamanhoAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new Tamanho
        {
            Id = r.GetInt32(0),
            Descricao = await r.GetNullableFieldValueAsync<string>(1, cancellationToken),
        };
    }


}