using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class StatusOrdemServicoDAO(Database database, ILogger<StatusOrdemServicoDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(StatusOrdemServico status, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO status_ordem_servico (descricao) VALUES ($1) RETURNING id",
            [status.Descricao],
            cancellationToken
        );
    }

    public async Task<StatusOrdemServico?> FindByIdAsync(int statusId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, descricao FROM status_ordem_servico WHERE id = $1",
            MapStatusOrdemServicoAsync,
            [statusId],
            cancellationToken
        );
    }

    public async Task<List<StatusOrdemServico>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, descricao FROM status_ordem_servico ORDER BY id",
            MapStatusOrdemServicoAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }
    
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync("DELETE FROM status_ordem_servico WHERE id = $1", [id], cancellationToken);
    }

    private static async Task<StatusOrdemServico> MapStatusOrdemServicoAsync(NpgsqlDataReader r,
        CancellationToken cancellationToken = default)
    {
        return new StatusOrdemServico
        {
            Id = r.GetInt32(0),
            Descricao = await r.GetNullableFieldValueAsync<string>(1, cancellationToken),
        };
    }
}