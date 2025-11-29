using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public class ItemOrdemServicoDAO(Database database) : DataAccessObjectBase(database)
{
    public async Task<int> InsertAsync(ItemOrdemServico item, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO item_ordem_servico (os_id, produto_variacao_id, quantidade_prevista, quantidade_usada)
            VALUES ($1, $2, $3, $4) RETURNING id
            """,
            [item.OrdemServicoId, item.ProdutoVariacaoId, item.QuantidadePrevista, item.QuantidadeUsada],
            cancellationToken
        );
    }

    public async Task<List<ItemOrdemServico>> ListByOrdemServicoAsync(int ordemServicoId, CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            """
            SELECT id, os_id, produto_variacao_id, quantidade_prevista, quantidade_usada
            FROM item_ordem_servico
            WHERE os_id = $1
            """,
            MapItemOrdemServicoAsync,
            [ordemServicoId],
            cancellationToken
        )).ToList();
    }

    private static async Task<ItemOrdemServico> MapItemOrdemServicoAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new ItemOrdemServico
        {
            Id = r.GetInt32(0),
            OrdemServicoId = await r.GetNullableFieldValueAsync<int>(1, cancellationToken),
            ProdutoVariacaoId = await r.GetNullableFieldValueAsync<int>(2, cancellationToken),
            QuantidadePrevista = await r.GetNullableFieldValueAsync<decimal>(3, cancellationToken),
            QuantidadeUsada = await r.GetNullableFieldValueAsync<decimal>(4, cancellationToken)
        };
    }

}