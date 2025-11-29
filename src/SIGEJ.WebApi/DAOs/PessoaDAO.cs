using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class PessoaDAO(Database database, ILogger<PessoaDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task<int> InsertAsync(Pessoa pessoa, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            """
            INSERT INTO pessoa (nome, cpf, matricula_siape, email, telefone, ativo)
            VALUES ($1, $2, $3, $4, $5, $6)
            RETURNING id
            """,
            [
                pessoa.Nome,
                pessoa.Cpf,
                pessoa.MatriculaSiape,
                pessoa.Email,
                pessoa.Telefone,
                pessoa.Ativo,
            ],
            cancellationToken
        );
    }

    public async Task<Pessoa?> FindByIdAsync(int pessoaId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, nome, cpf, matricula_siape, email, telefone, ativo FROM pessoa WHERE id = $1",
            MapPessoaAsync, [pessoaId], cancellationToken
        );
    }

    public async Task<List<Pessoa>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, nome, cpf, matricula_siape, email, telefone, ativo FROM pessoa ORDER BY id", MapPessoaAsync,
            cancellationToken: cancellationToken)).ToList();
    }

    public async Task UpdateAsync(Pessoa pessoa, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(
            """
            UPDATE pessoa
            SET nome = $1, cpf = $2, matricula_siape = $3, email = $4, telefone = $5, ativo = $6
            WHERE id = $7
            """,
            [
                pessoa.Nome,
                pessoa.Cpf,
                pessoa.MatriculaSiape,
                pessoa.Email,
                pessoa.Telefone,
                pessoa.Ativo,
                pessoa.Id,
            ], cancellationToken
        );
    }


    private static async Task<Pessoa> MapPessoaAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new Pessoa
        {
            Id = r.GetInt32(0),
            Nome = r.GetString(1),
            Cpf = await r.GetNullableFieldValueAsync<string>(2, cancellationToken),
            MatriculaSiape = await r.GetNullableFieldValueAsync<string>(3, cancellationToken),
            Email = await r.GetNullableFieldValueAsync<string>(4, cancellationToken),
            Telefone = await r.GetNullableFieldValueAsync<string>(5, cancellationToken),
            Ativo = await r.GetNullableFieldValueAsync<bool>(6, cancellationToken)
        };
    }
}