namespace SIGEJ.WebApi.Models;

public sealed class UnidadeMedida
{
    public int Id { get; set; }
    public string Sigla { get; set; } = null!;
    public string? Descricao { get; set; }
}