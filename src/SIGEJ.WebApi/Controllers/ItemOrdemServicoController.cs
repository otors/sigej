using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Ordem de Serviço")]
public sealed class ItemOrdemServicoController(ItemOrdemServicoDAO dao) : ControllerBase
{
    [HttpGet("{ordemServicoId:int}")]
    public async Task<ActionResult<IEnumerable<ItemOrdemServico>>> GetAllByOrdemServico(int ordemServicoId, CancellationToken cancellationToken = default)
    {
        var result = await dao.ListByOrdemServicoAsync(ordemServicoId, cancellationToken);
        return result;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ItemOrdemServico itemOrdemServico, CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(itemOrdemServico, cancellationToken);
        return Created();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}