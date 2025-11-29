using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public class TipoFuncionarioDAO(Database database) : DataAccessObjectBase(database)
{
    // public int Id { get; set; }
    // public string Descricao { get; set; } = null!;

    public async Task<int> InsertAsync(TipoFuncionario tipoFuncionario, CancellationToken cancellationToken = default)
    {
        return await ExecuteReturningIdAsync("INSERT INTO tipo_funcionario (descricao) VALUES ($1) RETURNING id",
            [tipoFuncionario.Descricao], cancellationToken);
    }

    public async Task<TipoFuncionario?> FindByIdAsync(int tipoFuncionarioId,
        CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync("SELECT id, descricao FROM tipo_funcionario WHERE id = $1", MapTipoFuncionarioAsync,
            [tipoFuncionarioId], cancellationToken);
    }
    
    public async Task<List<TipoFuncionario>> ListAllAsync(int tipoFuncionarioId,
        CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync("SELECT id, descricao FROM tipo_funcionario ORDER BY id", MapTipoFuncionarioAsync,
            cancellationToken: cancellationToken)).ToList();
    }

    private static Task<TipoFuncionario> MapTipoFuncionarioAsync(NpgsqlDataReader r,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TipoFuncionario
        {
            Id = r.GetInt32(0),
            Descricao = r.GetString(1),
        });
    }
}