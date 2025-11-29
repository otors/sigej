using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Equipe Manutenção")]
public sealed class EquipeMembroController(EquipeMembroDAO dao) : ControllerBase
{
    [HttpGet("{equipeId:int}")]
    public async Task<ActionResult<IEnumerable<EquipeMembro>>> GetAllAsync(int equipeId, CancellationToken cancellationToken = default)
    {
        var result = await dao.ListByEquipeAsync(equipeId, cancellationToken);
        return result;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] EquipeMembro equipeMembro, CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(equipeMembro, cancellationToken);
        return Created();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}