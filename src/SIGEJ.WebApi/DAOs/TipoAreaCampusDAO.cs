using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public class TipoAreaCampusDAO(Database database) : DataAccessObjectBase(database)
{
    public async Task<int> InsertAsync(TipoAreaCampus tipo, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO tipo_area_campus (descricao) VALUES ($1) RETURNING id",
            [tipo.Descricao], cancellationToken
        );
    }

    public async Task<TipoAreaCampus?> FindByIdAsync(int tipoId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, descricao FROM tipo_area_campus WHERE id = $1",
            MapTipoAreaCampusAsync,
            [tipoId],
            cancellationToken
        );
    }

    public async Task<List<TipoAreaCampus>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, descricao FROM tipo_area_campus ORDER BY descricao",
            MapTipoAreaCampusAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    public async Task DeleteAsync(int tipoId, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(
            "DELETE FROM tipo_area_campus WHERE id = $1",
            [tipoId],
            cancellationToken
        );
    }

    private static async Task<TipoAreaCampus> MapTipoAreaCampusAsync(NpgsqlDataReader r,
        CancellationToken cancellationToken = default)
    {
        return new TipoAreaCampus
        {
            Id = r.GetInt32(0),
            Descricao = await r.GetNullableFieldValueAsync<string>(1, cancellationToken)
        };
    }
}