namespace SIGEJ.WebApi.Models;

public sealed class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Cpf { get; set; }
    public string? MatriculaSiape { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public bool? Ativo { get; set; } = true;
}