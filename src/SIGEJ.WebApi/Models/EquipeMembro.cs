namespace SIGEJ.WebApi.Models;

public sealed class EquipeMembro
{
    public int Id { get; set; }
    public int? EquipeId { get; set; }
    public int? FuncionarioId { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public string? Funcao { get; set; }
}