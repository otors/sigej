namespace SIGEJ.Api.Models;

public class MovimentoEstoque
{
    public int Id { get; set; }
    public int ProdutoVariacaoId { get; set; }
    public ProdutoVariacao? ProdutoVariacao { get; set; }
    public int LocalEstoqueId { get; set; }
    public LocalEstoque? LocalEstoque { get; set; }
    public int TipoMovimentoId { get; set; }
    public TipoMovimentoEstoque? TipoMovimento { get; set; }
    public decimal Quantidade { get; set; }
    public DateTime DataHora { get; set; } = DateTime.Now;
    public int FuncionarioId { get; set; }
    public Funcionario? Funcionario { get; set; }
    public int OrdemServicoId { get; set; }
    public string Observacao { get; set; } = string.Empty;
    
    
}