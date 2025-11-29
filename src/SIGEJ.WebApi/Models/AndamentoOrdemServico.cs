namespace SIGEJ.WebApi.Models;

public sealed class AndamentoOrdemServico
{
    public int Id { get; set; }
    public int? OrdemServicoId { get; set; }
    public DateTime? DataHora { get; set; } = DateTime.Now;
    public int? StatusAnteriorId { get; set; }
    public int? StatusNovoId { get; set; }
    public int? FuncionarioId { get; set; }
    public string? Descricao { get; set; }
    public DateTime? InicioAtendimento { get; set; }
    public DateTime? FimAtendimento { get; set; }
}