namespace SIGEJ.WebApi.Models;

public sealed class Setor
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Sigla { get; set; }
}