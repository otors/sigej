namespace SIGEJ.WebApi.DTOs;

public sealed record MovimentoEstoqueDTO(
    int Id,
    DateTime DataHora,
    string TipoMovimento,
    char SinalTipoMovimento,
    string CodigoInterno,
    string Local,
    decimal Quantidade,
    int? FuncionarioId,
    int? OrdemServicoId,
    string Observacao);