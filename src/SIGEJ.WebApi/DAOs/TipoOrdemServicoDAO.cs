using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class TipoOrdemServicoDAO(Database database, ILogger<TipoOrdemServicoDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(TipoOrdemServico tipo, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO tipo_ordem_servico (descricao) VALUES ($1) RETURNING id",
            [tipo.Descricao],
            cancellationToken
        );
    }

    public async Task<TipoOrdemServico?> FindByIdAsync(int tipoId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, descricao FROM tipo_ordem_servico WHERE id = $1",
            MapTipoOrdemServicoAsync,
            [tipoId],
            cancellationToken
        );
    }

    public async Task<List<TipoOrdemServico>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, descricao FROM tipo_ordem_servico ORDER BY descricao",
            MapTipoOrdemServicoAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }
    
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync("DELETE FROM tipo_ordem_servico WHERE id = $1", [id], cancellationToken);
    }

    private static async Task<TipoOrdemServico> MapTipoOrdemServicoAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new TipoOrdemServico
        {
            Id = r.GetInt32(0),
            Descricao = await r.GetNullableFieldValueAsync<string>(1, cancellationToken),
        };

    }

}