using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Material")]
public sealed class FornecedorController(FornecedorDAO dao) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Fornecedor>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await dao.ListAllAsync(cancellationToken);
        return result;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Fornecedor fornecedor, CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(fornecedor, cancellationToken);
        return Created();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}