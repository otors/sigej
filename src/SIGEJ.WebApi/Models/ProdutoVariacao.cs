namespace SIGEJ.Api.Models;

public class ProdutoVariacao
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public int CorId { get; set; }
    public Cor? Cor { get; set; }
    public int TamanhoId { get; set; }
    public Tamanho? Tamanho { get; set; }
    public string CodigoBarras { get; set; } = string.Empty;
    public string CodigoInterno { get; set; } = string.Empty;
}