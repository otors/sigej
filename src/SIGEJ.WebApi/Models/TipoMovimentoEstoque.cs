namespace SIGEJ.WebApi.Models;

public sealed class TipoMovimentoEstoque
{
    public int Id { get; set; }
    public string? Descricao { get; set; }
    public char? Sinal { get; set; }
}