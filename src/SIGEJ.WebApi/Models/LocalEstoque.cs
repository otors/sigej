namespace SIGEJ.Api.Models;

public class LocalEstoque
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int ResponsavelId { get; set; }
    public Funcionario? Responsavel { get; set; }
}