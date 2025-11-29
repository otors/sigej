using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class ProdutoVariacaoDAO(Database database, ILogger<ProdutoVariacaoDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(ProdutoVariacao variacao, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO produto_variacao (produto_id, cor_id, tamanho_id, codigo_barras, codigo_interno)
            VALUES ($1, $2, $3, $4, $5)
            RETURNING id
            """,
            [
                variacao.ProdutoId,
                variacao.CorId,
                variacao.TamanhoId,
                variacao.CodigoBarras,
                variacao.CodigoInterno
            ],
            cancellationToken
        );
    }

    public async Task<ProdutoVariacao?> FindByIdAsync(int variacaoId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            """
            SELECT id, produto_id, cor_id, tamanho_id, codigo_barras, codigo_interno
            FROM produto_variacao WHERE id = $1
            """,
            MapProdutoVariacaoAsync,
            [variacaoId],
            cancellationToken
        );
    }

    public async Task<List<ProdutoVariacao>> ListByProdutoAsync(int produtoId, CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            """
            SELECT id, produto_id, cor_id, tamanho_id, codigo_barras, codigo_interno
            FROM produto_variacao WHERE produto_id = $1
            ORDER BY id
            """,
            MapProdutoVariacaoAsync,
            [produtoId],
            cancellationToken
        )).ToList();
    }

    public async Task<List<ProdutoVariacao>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            """
            SELECT id, produto_id, cor_id, tamanho_id, codigo_barras, codigo_interno
            FROM produto_variacao ORDER BY id
            """,
            MapProdutoVariacaoAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }
    
    
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync("DELETE FROM produto_variacao WHERE id = $1", [id], cancellationToken);
    }

    private static async Task<ProdutoVariacao> MapProdutoVariacaoAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new ProdutoVariacao
        {
            Id = r.GetInt32(0),
            ProdutoId = await r.GetNullableFieldValueAsync<int>(1, cancellationToken),
            CorId = await r.GetNullableFieldValueAsync<int>(2, cancellationToken),
            TamanhoId = await r.GetNullableFieldValueAsync<int>(3, cancellationToken),
            CodigoBarras = await r.GetNullableFieldValueAsync<string>(4, cancellationToken),
            CodigoInterno = await r.GetNullableFieldValueAsync<string>(5, cancellationToken)
        };
    }

}