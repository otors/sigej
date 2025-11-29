using System.Data;
using System.Data.Common;
using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.DTOs;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class AndamentoOrdemServicoDAO(Database database, ILogger<AndamentoOrdemServicoDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(AndamentoOrdemServico aos, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO andamento_ordem_servico (
                os_id, data_hora, status_anterior_id, status_novo_id, funcionario_id, descricao,
                inicio_atendimento, fim_atendimento
            ) VALUES ($1, $2, $3, $4, $5, $6, $7, $8)
            RETURNING id
            """, [
                aos.OrdemServicoId,
                aos.DataHora,
                aos.StatusAnteriorId,
                aos.StatusNovoId,
                aos.FuncionarioId,
                aos.Descricao,
                aos.InicioAtendimento,
                aos.FimAtendimento
            ], cancellationToken);
    }

    public async Task<List<TimelineDTO>> TimelineAsync(int ordemServicoId,
        CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            """
            SELECT a.data_hora, pes.nome AS funcionario, sts_novo.descricao AS status_atual, a.descricao
            FROM andamento_ordem_servico a
            JOIN funcionario f ON a.funcionario_id = f.id
            JOIN pessoa pes ON f.pessoa_id = pes.id
            JOIN status_ordem_servico sts_novo ON a.status_novo_id = sts_novo.id
            WHERE a.os_id = $1
            ORDER BY a.data_hora
            """, MapTimelineAsync, [ordemServicoId], cancellationToken
        )).ToList();
    }

    private static async Task<TimelineDTO> MapTimelineAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default) =>
        new(
            await r.GetNullableFieldValueAsync<DateTime>(0, cancellationToken),
            await r.GetNullableFieldValueAsync<string>(1, cancellationToken),
            await r.GetNullableFieldValueAsync<string>(2, cancellationToken),
            await r.GetNullableFieldValueAsync<string>(3, cancellationToken)
        );
    
}