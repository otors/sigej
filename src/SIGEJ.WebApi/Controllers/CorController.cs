using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Material")]
public sealed class CorController(CorDAO dao) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cor>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await dao.ListAllAsync(cancellationToken);
        return result;
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cor>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await dao.FindByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : result;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Cor cor, CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(cor, cancellationToken);
        return Created();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}