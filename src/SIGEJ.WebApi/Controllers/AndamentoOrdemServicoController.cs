using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;
using SIGEJ.WebApi.DTOs;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Ordem de Serviço")]
public sealed class AndamentoOrdemServicoController(AndamentoOrdemServicoDAO dao) : ControllerBase
{
    [HttpGet("{id:int}/timeline")]
    public async Task<ActionResult<List<TimelineDTO>>> GetTimelineAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await dao.TimelineAsync(id, cancellationToken);
        return result;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] AndamentoOrdemServico andamentoOrdemServico, CancellationToken cancellationToken = default)
    {
        
        await dao.InsertAsync(andamentoOrdemServico, cancellationToken);
        return Created();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}