namespace SIGEJ.WebApi.Models;

public sealed class EquipeManutencao
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Turno { get; set; }
}