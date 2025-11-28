namespace SIGEJ.Api.Models;

public class OrdemServico
{
    public int Id { get; set; }
    public string NumeroSequencial { get; set; } = string.Empty;
    public int SolicitanteId { get; set; }
    public Pessoa? Solicitante { get; set; }
    public int AreaCampusId { get; set; }
    public AreaCampus? AreaCampus { get; set; }
    public int TipoOrdemServicoId { get; set; }
    public TipoOrdemServico? TipoOrdemServico { get; set; }
    public int EquipeId { get; set; }
    public EquipeManutencao? Equipe { get; set; }
    public int LiderId { get; set; }
    public Funcionario? Lider { get; set; }
    public int StatusOrdemServicoId { get; set; } = 1;
    public StatusOrdemServico? StatusOrdemServico { get; set; }
    public int Prioridade { get; set; }
    public DateTime DataAbertura { get; set; } = DateTime.Now;
    public DateOnly DataPrevista { get; set; }
    public string DescricaoProblema { get; set; } = string.Empty;
    
}