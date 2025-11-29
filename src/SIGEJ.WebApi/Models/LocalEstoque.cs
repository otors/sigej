namespace SIGEJ.WebApi.Models;

public sealed class LocalEstoque
{
    public int Id { get; set; }
    public string? Descricao { get; set; }
    public int? ResponsavelId { get; set; }
}