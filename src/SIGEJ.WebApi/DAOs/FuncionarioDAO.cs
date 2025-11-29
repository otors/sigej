using System.Data;
using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class FuncionarioDAO(Database database, ILogger<FuncionarioDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(Funcionario funcionario, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO funcionario (pessoa_id, tipo_funcionario_id, setor_id, data_admissao, data_demissao)
            VALUES ($1, $2, $3, $4, $5)
            RETURNING id
            """, [
                funcionario.PessoaId,
                funcionario.TipoFuncionarioId,
                funcionario.SetorId,
                funcionario.DataAdmissao,
                funcionario.DataDemissao,
            ], cancellationToken);
    }

    public async Task<Funcionario?> FindByIdAsync(int funcionarioId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            """
            SELECT id, pessoa_id, tipo_funcionario_id, setor_id, data_admissao, data_demissao
            FROM funcionario WHERE id = $1
            """, MapFuncionarioAsync, [funcionarioId], cancellationToken);
    }

    public async Task<List<Funcionario>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            """
            SELECT id, pessoa_id, tipo_funcionario_id, setor_id, data_admissao, data_demissao
            FROM funcionario ORDER BY id
            """, MapFuncionarioAsync, cancellationToken: cancellationToken)).ToList();
    }

    public async Task DemitirAsync(int funcionarioId, DateOnly dataDemissao,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync("UPDATE funcionario SET data_demissao = $1 WHERE id = $2", [dataDemissao, funcionarioId],
            cancellationToken);
    }
    
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync("DELETE FROM funcionario WHERE id = $1", [id], cancellationToken);
    }
    
    private static async Task<Funcionario> MapFuncionarioAsync(NpgsqlDataReader r,
        CancellationToken cancellationToken = default) =>
        new()
        {
            Id = r.GetInt32(0),
            PessoaId = await r.GetNullableFieldValueAsync<int>(1, cancellationToken),
            TipoFuncionarioId = await r.GetNullableFieldValueAsync<int>(2, cancellationToken),
            SetorId = await r.GetNullableFieldValueAsync<int>(3, cancellationToken),
            DataAdmissao = await r.GetNullableFieldValueAsync<DateOnly>(4, cancellationToken),
            DataDemissao = await r.GetNullableFieldValueAsync<DateOnly>(5, cancellationToken)
        };
}