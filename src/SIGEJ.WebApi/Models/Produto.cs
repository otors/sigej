namespace SIGEJ.Api.Models;

public class Produto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = null!;
    public int CategoriaId { get; set; }
    public CategoriaMaterial? Categoria { get; set; }
    public int UnidadeMedidaId { get; set; }
    public UnidadeMedida? UnidadeMedida { get; set; }
    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }
}