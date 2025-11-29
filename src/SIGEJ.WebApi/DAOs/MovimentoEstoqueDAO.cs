using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.DTOs;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class MovimentoEstoqueDAO(Database database, ILogger<MovimentoEstoqueDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(MovimentoEstoque movimento, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO movimento_estoque (
                produto_variacao_id, local_estoque_id, tipo_movimento_id, quantidade,
                data_hora, funcionario_id, ordem_servico_id, observacao
            ) VALUES ($1, $2, $3, $4, $5, $6, $7, $8)
            RETURNING id
            """,
            [
                movimento.ProdutoVariacaoId,
                movimento.LocalEstoqueId,
                movimento.TipoMovimentoId,
                movimento.Quantidade,
                movimento.DataHora,
                movimento.FuncionarioId,
                movimento.OrdemServicoId,
                movimento.Observacao
            ],
            cancellationToken
        );
    }

    public async Task<List<MovimentoEstoqueDTO>> ListAllAsync(
        CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            """
            SELECT me.id, me.data_hora, tm.descricao, tm.sinal,
                   pv.codigo_interno, le.descricao AS local, me.quantidade,
                   me.funcionario_id, me.ordem_servico_id, me.observacao
            FROM movimento_estoque me
            JOIN tipo_movimento_estoque tm ON me.tipo_movimento_id = tm.id
            JOIN produto_variacao pv ON me.produto_variacao_id = pv.id
            JOIN local_estoque le ON me.local_estoque_id = le.id
            ORDER BY me.data_hora DESC
            """,
            MapMovimentoEstoqueAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    public async Task AtualizarDataEOsAsync(int movId, DateTime dataHora, int? osId,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(
            "UPDATE movimento_estoque SET data_hora = $1, ordem_servico_id = $2 WHERE id = $3",
            [dataHora, osId, movId],
            cancellationToken
        );
    }

    private static async Task<MovimentoEstoqueDTO> MapMovimentoEstoqueAsync(NpgsqlDataReader r,
        CancellationToken cancellationToken = default)
    {
        return new MovimentoEstoqueDTO(
            r.GetInt32(0),
            r.GetDateTime(1),
            r.GetString(2),
            r.GetChar(3),
            r.GetString(4),
            r.GetString(5),
            r.GetDecimal(6),
            await r.GetNullableFieldValueAsync<int>(7, cancellationToken),
            await r.GetNullableFieldValueAsync<int>(8, cancellationToken),
            r.GetString(9));
    }
}