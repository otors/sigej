namespace SIGEJ.Api.Models;

public class TipoMovimentoEstoque
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public char Sinal { get; set; }
}