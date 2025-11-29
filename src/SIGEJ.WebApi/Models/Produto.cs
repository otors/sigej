namespace SIGEJ.WebApi.Models;

public sealed class Produto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = null!;
    public int? CategoriaId { get; set; }
    public int? UnidadeMedidaId { get; set; }
    public int? MarcaId { get; set; }
}