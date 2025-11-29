using System.Data;
using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class EquipeManutencaoDAO(Database database) : DataAccessObjectBase(database)
{
    public async Task<int> InsertAsync(EquipeManutencao equipe, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO equipe_manutencao (nome, turno) VALUES ($1, $2) RETURNING id",
            [equipe.Nome, equipe.Turno],
            cancellationToken
        );
    }

    public async Task<EquipeManutencao?> FindByIdAsync(int equipeId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, nome, turno FROM equipe_manutencao WHERE id = $1",
            MapEquipeManutencaoAsync,
            [equipeId],
            cancellationToken
        );
    }

    public async Task<List<EquipeManutencao>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, nome, turno FROM equipe_manutencao ORDER BY nome",
            MapEquipeManutencaoAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<EquipeManutencao> MapEquipeManutencaoAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new EquipeManutencao
        {
            Id = r.GetInt32(0),
            Nome = r.GetString(1),
            Turno = await r.GetNullableFieldValueAsync<string>(2, cancellationToken),
        };
    }

}