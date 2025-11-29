using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class TipoMovimentoEstoqueDAO(Database database) : DataAccessObjectBase(database)
{
    public async Task<int> InsertAsync(TipoMovimentoEstoque tipo, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO tipo_movimento_estoque (descricao, sinal) VALUES ($1, $2) RETURNING id",
            [tipo.Descricao, tipo.Sinal],
            cancellationToken
        );
    }

    public async Task<TipoMovimentoEstoque?> FindByIdAsync(int tipoId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, descricao, sinal FROM tipo_movimento_estoque WHERE id = $1",
            MapTipoMovimentoEstoqueAsync,
            [tipoId],
            cancellationToken
        );
    }

    public async Task<List<TipoMovimentoEstoque>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, descricao, sinal FROM tipo_movimento_estoque ORDER BY id",
            MapTipoMovimentoEstoqueAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<TipoMovimentoEstoque> MapTipoMovimentoEstoqueAsync(NpgsqlDataReader r,
        CancellationToken cancellationToken = default)
    {
        return new TipoMovimentoEstoque
        {
            Id = r.GetInt32(0),
            Descricao = await r.GetNullableFieldValueAsync<string>(1, cancellationToken),
            Sinal = await r.GetNullableFieldValueAsync<char>(2, cancellationToken)
        };
    }
}