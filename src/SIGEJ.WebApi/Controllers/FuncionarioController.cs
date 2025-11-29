using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Funcionario")]
public sealed class FuncionarioController(FuncionarioDAO dao) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Funcionario>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await dao.ListAllAsync(cancellationToken);
        return result;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Funcionario>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await dao.FindByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Funcionario funcionario,
        CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(funcionario, cancellationToken);
        return Created();
    }

    [HttpPost("{id:int}/demitir")]
    public async Task<ActionResult<Funcionario>> DismissAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DemitirAsync(id, DateOnly.FromDateTime(DateTime.Now), cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}