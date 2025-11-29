namespace SIGEJ.WebApi.Models;

public sealed class Estoque
{
    public int ProdutoVariacaoId { get; set; }
    public int LocalEstoqueId { get; set; }
    public decimal? Quantidade { get; set; } = 0;
    public decimal? PontoReposicao { get; set; } = 0;

}