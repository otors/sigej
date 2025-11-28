namespace SIGEJ.Api.Models;

public class UnidadeMedida
{
    public int Id { get; set; }
    public string Sigla { get; set; } = null!;
    public string Descricao { get; set; } = string.Empty;
}