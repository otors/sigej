using Microsoft.AspNetCore.Mvc;
using SIGEJ.WebApi.Models;
using SIGEJ.WebApi.DAOs;
using SIGEJ.WebApi.DTOs.Requests;

namespace SIGEJ.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Ordem de Serviço")]
public sealed class OrdemServicoController(OrdemServicoDAO dao) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrdemServico>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await dao.ListAllAsync(cancellationToken);
        return result;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrdemServico>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await dao.FindByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] OrdemServico ordemServico,
        CancellationToken cancellationToken = default)
    {
        await dao.InsertAsync(ordemServico, cancellationToken);
        return Created();
    }

    [HttpPatch("{id:int}/data-abertura")]
    public async Task<ActionResult<OrdemServico>> UpdateDataAberturaAsync(int id, AtualizarDataAberturaRequest request,
        CancellationToken cancellationToken = default)
    {
        await dao.UpdateDataAberturaAsync(id, request.DataAbertura, cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<OrdemServico>> UpdateStatusAsync(int id, AtualizarStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        await dao.UpdateStatusAsync(id, request.NovoStatusId, cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await dao.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}