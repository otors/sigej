using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public class UnidadeMedidaDAO(Database database) : DataAccessObjectBase(database)
{
    public async Task<int> InsertAsync(UnidadeMedida unidade, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO unidade_medida (sigla, descricao) VALUES ($1, $2) RETURNING id",
            [unidade.Sigla, unidade.Descricao],
            cancellationToken
        );
    }

    public async Task<UnidadeMedida?> FindByIdAsync(int unidadeId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, sigla, descricao FROM unidade_medida WHERE id = $1",
            MapUnidadeMedidaAsync,
            [unidadeId],
            cancellationToken
        );
    }

    public async Task<List<UnidadeMedida>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, sigla, descricao FROM unidade_medida ORDER BY sigla",
            MapUnidadeMedidaAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<UnidadeMedida> MapUnidadeMedidaAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new UnidadeMedida
        {
            Id = r.GetInt32(0),
            Sigla = r.GetString(1),
            Descricao = await r.GetNullableFieldValueAsync<string>(2, cancellationToken)
        };
    }

}