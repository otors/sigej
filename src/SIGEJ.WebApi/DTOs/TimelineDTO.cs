namespace SIGEJ.WebApi.DTOs;

public sealed record TimelineDTO(DateTime? DataHora, string? Funcionario, string? StatusAtual, string? Descricao);