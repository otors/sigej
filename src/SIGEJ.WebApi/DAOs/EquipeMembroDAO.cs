using System.Data;
using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class EquipeMembroDAO(Database database, ILogger<EquipeMembroDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(EquipeMembro membro, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO equipe_membro (equipe_id, funcionario_id, data_inicio, data_fim, funcao)
            VALUES ($1, $2, $3, $4, $5) RETURNING id
            """,
            [membro.EquipeId, membro.FuncionarioId, membro.DataInicio, membro.DataFim, membro.Funcao],
            cancellationToken
        );
    }

    public async Task<List<EquipeMembro>> ListByEquipeAsync(int equipeId, CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            """
            SELECT id, equipe_id, funcionario_id, data_inicio, data_fim, funcao
            FROM equipe_membro WHERE equipe_id = $1
            ORDER BY data_inicio DESC
            """,
            MapEquipeMembroAsync,
            [equipeId],
            cancellationToken
        )).ToList();
    }

    private static async Task<EquipeMembro> MapEquipeMembroAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new EquipeMembro
        {
            Id = r.GetInt32(0),
            EquipeId = await r.GetNullableFieldValueAsync<int>(1, cancellationToken),
            FuncionarioId = await r.GetNullableFieldValueAsync<int>(2, cancellationToken),
            DataInicio = await r.GetFieldValueAsync<DateOnly>(3, cancellationToken),
            DataFim = await r.GetNullableFieldValueAsync<DateOnly>(4, cancellationToken),
            Funcao = await r.GetNullableFieldValueAsync<string>(5, cancellationToken)
        };
    }

}