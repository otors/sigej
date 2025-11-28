namespace SIGEJ.Api.Models;

public class EquipeMembro
{
    public int Id { get; set; }
    public int EquipeId { get; set; }
    public EquipeManutencao? Equipe { get; set; }
    public int FuncionarioId { get; set; }
    public Funcionario? Funcionario { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public string Funcao { get; set; } = string.Empty;
}