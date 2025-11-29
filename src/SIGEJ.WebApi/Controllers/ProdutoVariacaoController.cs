using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Produto")]
public sealed class ProdutoVariacaoController(ProdutoVariacaoDAO dao) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoVariacao>>> GetAllAsync([FromQuery] int? produtoId,
        CancellationToken cancellationToken = default)
    {
        var result = produtoId is null
            ? await dao.ListAllAsync(cancellationToken)
            : await dao.ListByProdutoAsync(produtoId.Value, cancellationToken);
        return result;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProdutoVariacao>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await dao.FindByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ProdutoVariacao produtoVariacao,
        CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(produtoVariacao, cancellationToken);
        return Created();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}