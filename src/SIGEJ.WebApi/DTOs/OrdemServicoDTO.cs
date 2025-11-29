namespace SIGEJ.WebApi.DTOs;

public record OrdemServicoDTO(
    int Id,
    string NumeroSequencial,
    int Prioridade,
    string Area,
    string TipoServico,
    string Solicitante,
    DateTime DataAbertura
);