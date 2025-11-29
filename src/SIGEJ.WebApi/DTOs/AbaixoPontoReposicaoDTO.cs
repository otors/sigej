namespace SIGEJ.WebApi.DTOs;

public sealed record AbaixoPontoReposicaoDTO(
    string Produto,
    string CodigoInterno,
    string LocalEstoque,
    decimal Quantidade,
    decimal PontoReposicao);