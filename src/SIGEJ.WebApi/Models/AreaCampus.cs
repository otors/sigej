namespace SIGEJ.WebApi.Models;

public sealed class AreaCampus
{
    public int Id { get; set; }
    public int? TipoAreaId { get; set; }
    public string Descricao { get; set; } = null!;
    public string? Bloco { get; set; }
}