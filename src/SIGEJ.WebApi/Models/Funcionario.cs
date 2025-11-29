namespace SIGEJ.WebApi.Models;

public sealed class Funcionario
{
    public int Id { get; set; }
    public int? PessoaId { get; set; }
    public int? TipoFuncionarioId { get; set; }
    public int? SetorId { get; set; }
    public DateOnly? DataAdmissao { get; set; }
    public DateOnly? DataDemissao { get; set; }
}