using System.Data;
using Npgsql;
using SIGEJ.WebApi.Abstractions;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.DTOs;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.DAOs;

public sealed class EstoqueDAO(Database database, ILogger<EstoqueDAO> logger) : DataAccessObjectBase(database, logger)
{
    public async Task UpsertAsync(Estoque estoque, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(
            """
            INSERT INTO estoque (produto_variacao_id, local_estoque_id, quantidade, ponto_reposicao)
            VALUES ($1, $2, $3, $4)
            ON CONFLICT (produto_variacao_id, local_estoque_id)
            DO UPDATE SET quantidade = EXCLUDED.quantidade,
                          ponto_reposicao = EXCLUDED.ponto_reposicao
            """,
            [
                estoque.ProdutoVariacaoId,
                estoque.LocalEstoqueId,
                estoque.Quantidade,
                estoque.PontoReposicao
            ],
            cancellationToken
        );
    }

    public async Task AjustarQuantidadeAsync(int produtoVariacaoId, int localId, decimal delta,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(
            """
            UPDATE estoque
            SET quantidade = quantidade + $1
            WHERE produto_variacao_id = $2 AND local_estoque_id = $3
            """,
            [delta, produtoVariacaoId, localId],
            cancellationToken
        );
    }

    public async Task<Estoque?> FindAsync(int produtoVariacaoId, int localId,
        CancellationToken cancellationToken = default)
    {
        return await FetchOneAsync(
            """
            SELECT produto_variacao_id, local_estoque_id, quantidade, ponto_reposicao
            FROM estoque
            WHERE produto_variacao_id = $1 AND local_estoque_id = $2
            """,
            MapEstoqueAsync,
            [produtoVariacaoId, localId],
            cancellationToken
        );
    }

    public async Task<List<AbaixoPontoReposicaoDTO>>
        ListAbaixoPontoReposicaoAsync(CancellationToken cancellationToken = default)
    {
        return (await FetchAllAsync(
            """
            SELECT p.descricao, pv.codigo_interno, le.descricao, e.quantidade, e.ponto_reposicao
            FROM estoque e
            JOIN produto_variacao pv ON e.produto_variacao_id = pv.id
            JOIN produto p ON pv.produto_id = p.id
            JOIN local_estoque le ON e.local_estoque_id = le.id
            WHERE e.quantidade < e.ponto_reposicao
            """,
            MapAbaixoPontoReposicaoAsync,
            cancellationToken: cancellationToken
        )).ToList();
    }

    private static Task<Estoque> MapEstoqueAsync(NpgsqlDataReader r,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Estoque
        {
            ProdutoVariacaoId = r.GetInt32(0),
            LocalEstoqueId = r.GetInt32(1),
            Quantidade = r.GetDecimal(2),
            PontoReposicao = r.GetDecimal(3)
        });
    }

    private static Task<AbaixoPontoReposicaoDTO> MapAbaixoPontoReposicaoAsync(
        NpgsqlDataReader r,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AbaixoPontoReposicaoDTO(
            r.GetString(0),
            r.GetString(1),
            r.GetString(2),
            r.GetDecimal(3),
            r.GetDecimal(4)));
    }
}