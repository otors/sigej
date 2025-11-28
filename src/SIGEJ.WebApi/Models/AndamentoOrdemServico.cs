namespace SIGEJ.Api.Models;

public class AndamentoOrdemServico
{
    public int Id { get; set; }
    public int OrdemServicoId { get; set; }
    public OrdemServico? OrdemServico { get; set; }
    public DateTime DataHora { get; set; } = DateTime.Now;
    public int StatusAnteriorId { get; set; }
    public StatusOrdemServico? StatusAnterior { get; set; }
    public int StatusNovoId { get; set; }
    public StatusOrdemServico? StatusNovo { get; set; }
    public int FuncionarioId {get; set;}
    public Funcionario? Funcionario { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime InicioAtendimento { get; set; }
    public DateTime FimAtendimento { get; set; }
    
}