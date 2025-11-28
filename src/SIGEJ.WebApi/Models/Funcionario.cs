namespace SIGEJ.Api.Models;

public sealed class Funcionario
{
    public int Id { get; set; }
    public int PessoaId { get; set; }
    public Pessoa? Pessoa { get; set; } 
    public int TipoFuncionarioId { get; set; }
    public TipoFuncionario? TipoFuncionario { get; set; }
    public Setor? Setor { get; set; }
    public DateOnly DataAdmissao { get; set; }
    public DateOnly DataDemissao { get; set; }
    
}