namespace SIGEJ.Api.Models;

public sealed class AreaCampus
{
    public int Id { get; set; }
    public int TipoAreaId { get; set; }
    public TipoAreaCampus? TipoArea { get; set; }
    public string Descricao { get; set; } = null!;
    public string Bloco { get; set; } = string.Empty;
}