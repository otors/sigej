using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public class LocalEstoqueDAO(Database database, ILogger<LocalEstoqueDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(LocalEstoque local, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO local_estoque (descricao, responsavel_id) VALUES ($1, $2) RETURNING id",
            [local.Descricao, local.ResponsavelId],
            cancellationToken
        );
    }

    public async Task<LocalEstoque?> FindByIdAsync(int localId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, descricao, responsavel_id FROM local_estoque WHERE id = $1",
            MapLocalEstoqueAsync,
            [localId],
            cancellationToken
        );
    }

    public async Task<List<LocalEstoque>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, descricao, responsavel_id FROM local_estoque ORDER BY descricao",
            MapLocalEstoqueAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }
    
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync("DELETE FROM local_estoque WHERE id = $1", [id], cancellationToken);
    }

    private static async Task<LocalEstoque> MapLocalEstoqueAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new LocalEstoque
        {
            Id = r.GetInt32(0),
            Descricao = await r.GetNullableFieldValueAsync<string>(1, cancellationToken),
            ResponsavelId = await r.GetNullableFieldValueAsync<int>(2, cancellationToken)
        };

    }

}