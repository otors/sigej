using System.Data;
using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class AreaCampusDAO(Database database, ILogger<AreaCampusDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(AreaCampus area, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO area_campus (tipo_area_id, descricao, bloco)
            VALUES ($1, $2, $3) RETURNING id
            """,
            [area.TipoAreaId, area.Descricao, area.Bloco],
            cancellationToken
        );
    }

    public async Task<AreaCampus?> FindByIdAsync(int areaId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, tipo_area_id, descricao, bloco FROM area_campus WHERE id = $1",
            MapAreaCampusAsync,
            [areaId],
            cancellationToken
        );
    }

    public async Task<List<AreaCampus>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, tipo_area_id, descricao, bloco FROM area_campus ORDER BY descricao",
            MapAreaCampusAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<AreaCampus> MapAreaCampusAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new AreaCampus
        {
            Id = r.GetInt32(0),
            TipoAreaId = await r.GetFieldValueAsync<int>(1, cancellationToken),
            Descricao = r.GetString(2),
            Bloco = await r.GetFieldValueAsync<string>(3, cancellationToken),
        };
    }

}