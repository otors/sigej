using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;
using SIGEJ.WebApi.DTOs;
using SIGEJ.WebApi.DTOs.Requests;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Estoque")]
public sealed class EstoqueController(EstoqueDAO dao) : ControllerBase
{
    [HttpGet("{produtoVariacaoId:int}/{localId:int}")]
    public async Task<ActionResult<Estoque>> GetByIdAsync(int produtoVariacaoId, int localId, CancellationToken cancellationToken = default)
    {
        var result = await dao.FindAsync(produtoVariacaoId, localId, cancellationToken);
        return result is null ? NotFound() : result;
    }
    
    [HttpGet("abaixo-ponto-reposicao")]
    public async Task<ActionResult<IEnumerable<AbaixoPontoReposicaoDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await dao.ListAbaixoPontoReposicaoAsync(cancellationToken);
        return result;
    }
    
    [HttpPut("{produtoVariacaoId:int}/{localId:int}")]
    public async Task<IActionResult> CreateAsync(int produtoVariacaoId, int localId, [FromBody] Estoque estoque, CancellationToken cancellationToken = default)
    {
        if (estoque.ProdutoVariacaoId != produtoVariacaoId || estoque.LocalEstoqueId != localId) return BadRequest();
        await dao.UpsertAsync(estoque, cancellationToken);
        return Created();
    }
    
    [HttpPatch("{produtoVariacaoId:int}/{localId:int}")]
    public async Task<ActionResult<Estoque>> AdjustAmountAsync(int produtoVariacaoId, int localId, [FromBody] AjustarQuantidadeRequest request, CancellationToken cancellationToken = default)
    {
        await dao.AjustarQuantidadeAsync(produtoVariacaoId, localId, request.DeltaQuantidade, cancellationToken);
        return await GetByIdAsync(produtoVariacaoId, localId, cancellationToken);
    }
    
    [HttpDelete("{produtoVariacaoId:int}/{localId:int}")]
    public async Task<IActionResult> DeleteAsync(int produtoVariacaoId, int localId, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(produtoVariacaoId, localId, cancellationToken);
        return NoContent();
    }
}