using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public class OrdemServicoDAO(Database database) : DataAccessObjectBase(database)
{
    public async Task<int> InsertAsync(OrdemServico os, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO ordem_servico (
                numero_sequencial, solicitante_id, area_campus_id, tipo_os_id, equipe_id,
                lider_id, status_id, prioridade, data_abertura, data_prevista, descricao_problema
            ) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11)
            RETURNING id
            """,
            [
                os.NumeroSequencial,
                os.SolicitanteId,
                os.AreaCampusId,
                os.TipoOrdemServicoId,
                os.EquipeId,
                os.LiderId,
                os.StatusOrdemServicoId,
                os.Prioridade,
                os.DataAbertura,
                os.DataPrevista,
                os.DescricaoProblema
            ],
            cancellationToken
        );
    }

    public async Task<OrdemServico?> FindByIdAsync(int osId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            """
            SELECT id, numero_sequencial, solicitante_id, area_campus_id, tipo_os_id,
                   equipe_id, lider_id, status_id, prioridade, data_abertura, data_prevista, descricao_problema
            FROM ordem_servico
            WHERE id = $1
            """,
            MapOrdemServicoAsync,
            [osId],
            cancellationToken
        );
    }

    public async Task UpdateStatusAsync(int osId, int novoStatusId, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(
            "UPDATE ordem_servico SET status_id = $1 WHERE id = $2",
            [novoStatusId, osId],
            cancellationToken
        );
    }

    public async Task UpdateDataAberturaAsync(int osId, DateTime dataAbertura,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(
            "UPDATE ordem_servico SET data_abertura = $1 WHERE id = $2",
            [dataAbertura, osId],
            cancellationToken
        );
    }

    public async Task<List<OrdemServico>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            """
            SELECT id, numero_sequencial, solicitante_id, area_campus_id, tipo_os_id,
                   equipe_id, lider_id, status_id, prioridade, data_abertura, data_prevista, descricao_problema
            FROM ordem_servico
            ORDER BY data_abertura DESC
            """,
            MapOrdemServicoAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<OrdemServico> MapOrdemServicoAsync(NpgsqlDataReader r,
        CancellationToken cancellationToken = default)
    {
        return new OrdemServico
        {
            Id = r.GetInt32(0),
            NumeroSequencial = await r.GetNullableFieldValueAsync<string>(1, cancellationToken),
            SolicitanteId = await r.GetNullableFieldValueAsync<int>(2, cancellationToken),
            AreaCampusId = await r.GetNullableFieldValueAsync<int>(3, cancellationToken),
            TipoOrdemServicoId = await r.GetNullableFieldValueAsync<int>(4, cancellationToken),
            EquipeId = await r.GetNullableFieldValueAsync<int>(5, cancellationToken),
            LiderId = await r.GetNullableFieldValueAsync<int>(6, cancellationToken),
            StatusOrdemServicoId = await r.GetNullableFieldValueAsync<int>(7, cancellationToken),
            Prioridade = await r.GetNullableFieldValueAsync<int>(8, cancellationToken),
            DataAbertura = await r.GetNullableFieldValueAsync<DateTime>(9, cancellationToken),
            DataPrevista = await r.GetNullableFieldValueAsync<DateOnly>(10, cancellationToken),
            DescricaoProblema = await r.GetNullableFieldValueAsync<string>(11, cancellationToken)
        };
    }
}