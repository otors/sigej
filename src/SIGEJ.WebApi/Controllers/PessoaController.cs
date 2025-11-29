using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Pessoa")]
public sealed class PessoaController(PessoaDAO dao) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pessoa>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await dao.ListAllAsync(cancellationToken);
        return result;
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Pessoa>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await dao.FindByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : result;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Pessoa pessoa, CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(pessoa, cancellationToken);
        return Created();
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Pessoa>> UpdateAsync(int id, [FromBody] Pessoa pessoa, CancellationToken cancellationToken = default)
    {
        if (id != pessoa.Id) return BadRequest();
        await dao.UpdateAsync(pessoa, cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}