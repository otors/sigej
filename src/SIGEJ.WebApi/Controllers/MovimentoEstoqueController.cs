using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;
using SIGEJ.WebApi.DTOs;
using SIGEJ.WebApi.DTOs.Requests;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Estoque")]
public sealed class MovimentoEstoqueController(MovimentoEstoqueDAO dao) : ControllerBase
{
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimentoEstoqueDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await dao.ListAllAsync(cancellationToken);
        return result;
    }
    
    [HttpGet("consumo-equipe")]
    public async Task<ActionResult<IEnumerable<ConsumoEquipeDTO>>> GetConsumoPorEquipeAsync([FromQuery] DateOnly dataInicio, [FromQuery] DateOnly dataFim, CancellationToken cancellationToken = default)
    {
        var result = await dao.ListConsumoEquipeAsync(dataInicio, dataFim, cancellationToken);
        return result;
    }
    
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<MovimentoEstoque>> UpdateDateAndOsAsync(int id, [FromBody] AtualizarDataEOsRequest request, CancellationToken cancellationToken = default)
    {
        await dao.AtualizarDataEOsAsync(id, request.Data, request.OrdemServicoId, cancellationToken);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] MovimentoEstoque movimentoEstoque, CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(movimentoEstoque, cancellationToken);
        return Created();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}