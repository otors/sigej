namespace SIGEJ.WebApi.Models;

public sealed class OrdemServico
{
    public int Id { get; set; }
    public string? NumeroSequencial { get; set; }
    public int? SolicitanteId { get; set; }
    public int? AreaCampusId { get; set; }
    public int? TipoOrdemServicoId { get; set; }
    public int? EquipeId { get; set; }
    public int? LiderId { get; set; }
    public int? StatusOrdemServicoId { get; set; } = 1;
    public int? Prioridade { get; set; }
    public DateTime? DataAbertura { get; set; } = DateTime.Now;
    public DateOnly? DataPrevista { get; set; }
    public string? DescricaoProblema { get; set; }

}