namespace SIGEJ.WebApi.Models;

public sealed class ProdutoVariacao
{
    public int Id { get; set; }
    public int? ProdutoId { get; set; }
    public int? CorId { get; set; }
    public int? TamanhoId { get; set; }
    public string? CodigoBarras { get; set; }
    public string? CodigoInterno { get; set; }
}