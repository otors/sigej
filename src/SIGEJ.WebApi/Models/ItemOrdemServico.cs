namespace SIGEJ.Api.Models;

public class ItemOrdemServico
{
    public int Id { get; set; }
    public int OrdemServicoId { get; set; }
    public OrdemServico? OrdemServico { get; set; }
    public int ProdutoVariacaoId { get; set; }
    public ProdutoVariacao? ProdutoVariacao { get; set; }
    public decimal QuantidadePrevista { get; set; }
    public decimal QuantidadeUsada { get; set; }
    
}