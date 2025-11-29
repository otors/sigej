using System.Data;
using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class FornecedorDAO(Database database) : DataAccessObjectBase(database)
{
    public async Task<int> InsertAsync(Fornecedor fornecedor, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO fornecedor (nome, cnpj) VALUES ($1, $2) RETURNING id",
            [fornecedor.Nome, fornecedor.Cnpj],
            cancellationToken
        );
    }

    public async Task<List<Fornecedor>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, nome, cnpj FROM fornecedor ORDER BY nome",
            MapFornecedorAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<Fornecedor> MapFornecedorAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new Fornecedor
        {
            Id = r.GetInt32(1),
            Nome = await r.GetNullableFieldValueAsync<string>(2, cancellationToken),
            Cnpj = await r.GetNullableFieldValueAsync<string>(3, cancellationToken),
        };
    }

}