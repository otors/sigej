using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public class MarcaDAO(Database database) : DataAccessObjectBase(database)
{
    public async Task<int> InsertAsync(Marca marca, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO marca (nome) VALUES ($1) RETURNING id",
            [marca.Nome],
            cancellationToken
        );
    }

    public async Task<Marca?> FindByIdAsync(int marcaId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, nome FROM marca WHERE id = $1",
            MapMarcaAsync,
            [marcaId],
            cancellationToken
        );
    }

    public async Task<List<Marca>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, nome FROM marca ORDER BY nome",
            MapMarcaAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<Marca> MapMarcaAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new Marca
        {
            Id = r.GetInt32(0),
            Nome = await r.GetNullableFieldValueAsync<string>(1, cancellationToken)
        };
    }

}