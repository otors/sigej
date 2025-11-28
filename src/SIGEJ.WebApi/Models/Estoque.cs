namespace SIGEJ.Api.Models;

public class Estoque
{
    public int ProdutoVariacaoId { get; set; }
    public ProdutoVariacao? ProdutoVariacao { get; set; }
    public int LocalEstoqueId { get; set; }
    public LocalEstoque? LocalEstoque { get; set; }
    public decimal Quantidade { get; set; } = 0;
    public decimal PontoReposicao { get; set; } = 0;

}