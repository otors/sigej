using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class ProdutoDAO(Database database, ILogger<ProdutoDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO produto (descricao, categoria_id, unidade_medida_id, marca_id)
            VALUES ($1, $2, $3, $4)
            RETURNING id
            """,
            [produto.Descricao, produto.CategoriaId, produto.UnidadeMedidaId, produto.MarcaId],
            cancellationToken
        );
    }

    public async Task<Produto?> FindByIdAsync(int produtoId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, descricao, categoria_id, unidade_medida_id, marca_id FROM produto WHERE id = $1",
            MapProdutoAsync,
            [produtoId],
            cancellationToken
        );
    }

    public async Task<List<Produto>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, descricao, categoria_id, unidade_medida_id, marca_id FROM produto ORDER BY descricao",
            MapProdutoAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<Produto> MapProdutoAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new Produto
        {
            Id = r.GetInt32(0),
            Descricao = r.GetString(1),
            CategoriaId = await r.GetNullableFieldValueAsync<int>(2, cancellationToken),
            UnidadeMedidaId = await r.GetNullableFieldValueAsync<int>(2, cancellationToken),
            MarcaId = await r.GetNullableFieldValueAsync<int>(2, cancellationToken)
        };
    }

}