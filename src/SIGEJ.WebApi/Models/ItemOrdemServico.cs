namespace SIGEJ.WebApi.Models;

public sealed class ItemOrdemServico
{
    public int Id { get; set; }
    public int? OrdemServicoId { get; set; }
    public int? ProdutoVariacaoId { get; set; }
    public decimal? QuantidadePrevista { get; set; }
    public decimal? QuantidadeUsada { get; set; }
    
}