using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Funcionário")]
public sealed class SetorController(SetorDAO dao) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Setor>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await dao.ListAllAsync(cancellationToken);
        return result;
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Setor>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await dao.FindByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : result;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Setor setor, CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(setor, cancellationToken);
        return Created();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}