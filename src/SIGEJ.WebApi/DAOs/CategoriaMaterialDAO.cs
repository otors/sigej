using System.Data;
using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class CategoriaMaterialDAO(Database database) : DataAccessObjectBase(database)
{
    public async Task<int> InsertAsync(CategoriaMaterial categoria, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync(
            "INSERT INTO categoria_material (nome) VALUES ($1) RETURNING id",
            [categoria.Nome],
            cancellationToken
        );
    }

    public async Task<CategoriaMaterial?> FindByIdAsync(int categoriaId, CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            "SELECT id, nome FROM categoria_material WHERE id = $1",
            MapCategoriaMaterialAsync,
            [categoriaId],
            cancellationToken
        );
    }

    public async Task<List<CategoriaMaterial>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            "SELECT id, nome FROM categoria_material ORDER BY nome",
            MapCategoriaMaterialAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static async Task<CategoriaMaterial> MapCategoriaMaterialAsync(NpgsqlDataReader r, CancellationToken cancellationToken = default)
    {
        return new CategoriaMaterial
        {
            Id = r.GetInt32(0),
            Nome = r.GetString(1),
        };
    }

}