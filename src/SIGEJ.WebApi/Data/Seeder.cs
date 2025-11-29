using Bogus;
using Bogus.Extensions.Brazil;
using Npgsql;
using SIGEJ.WebApi.DAOs;
using SIGEJ.WebApi.Models;

namespace SIGEJ.WebApi.Data;

public sealed class Seeder(Database db, IServiceProvider serviceProvider)
{
    private readonly Faker _faker = new("pt_BR");
    private readonly Random _random = new();

    public async Task RunAsync()
    {
        // Limpa todas as tabelas
        await TruncateTablesAsync();

        // Tipos de funcionário, setores e pessoas
        var tiposFunc = await SeedTiposFuncionarioAsync();
        var setores = await SeedSetoresAsync();
        var pessoas = await SeedPessoasAsync();
        var funcionarios = await SeedFuncionariosAsync(pessoas, tiposFunc, setores);

        // Áreas e equipes
        var tiposArea = await SeedTiposAreaAsync();
        var areas = await SeedAreasAsync(tiposArea);
        var equipes = await SeedEquipesAsync();
        await SeedEquipeMembrosAsync(equipes, funcionarios);

        // Produtos e categorias
        var categorias = await SeedCategoriasAsync();
        var unidades = await SeedUnidadesAsync();
        await SeedFornecedoresAsync();
        var marcas = await SeedMarcasAsync();
        var cores = await SeedCoresAsync();
        var tamanhos = await SeedTamanhosAsync();
        var produtos = await SeedProdutosAsync(categorias, unidades, marcas);
        var variacoes = await SeedVariacoesAsync(produtos, cores, tamanhos);

        // Ordens de serviço
        var tiposOs = await SeedTiposOsAsync();
        var statusOs = await SeedStatusOsAsync();
        var ordens = await SeedOrdensServicoAsync(
            solicitantesIds: pessoas,
            areasIds: areas,
            tiposOsIds: tiposOs,
            equipesIds: equipes,
            lideresIds: funcionarios,
            variacoesIds: variacoes,
            statusMap: statusOs
        );

        // Estoque e movimentos
        var tipoMovimentos = await SeedTiposMovimentoAsync();
        var locais = await SeedLocaisAsync(funcionarios);
        var estoquePairs = await SeedEstoqueAsync(variacoes, locais, tipoMovimentos, funcionarios, ordens);
        await SeedMovimentosExtrasAsync(estoquePairs, tipoMovimentos, funcionarios, ordens);

        // Andamentos das OS
        await SeedAndamentosAsync(ordens, statusOs, funcionarios);
        
        Console.WriteLine("Base de dados populada com sucesso.");
    }

    private async Task TruncateTablesAsync()
    {
        var tables = new[]
        {
            "andamento_ordem_servico",
            "item_ordem_servico",
            "ordem_servico",
            "movimento_estoque",
            "estoque",
            "local_estoque",
            "produto_variacao",
            "tamanho",
            "cor",
            "produto",
            "marca",
            "unidade_medida",
            "categoria_material",
            "fornecedor",
            "equipe_membro",
            "equipe_manutencao",
            "funcionario",
            "pessoa",
            "tipo_funcionario",
            "setor",
            "area_campus",
            "tipo_area_campus",
            "tipo_movimento_estoque",
            "tipo_ordem_servico",
            "status_ordem_servico"
        };

        var sql = $"TRUNCATE {string.Join(", ", tables)} RESTART IDENTITY CASCADE;";

        Console.WriteLine(sql);

        await using var conn = db.GetConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<List<int>> SeedTiposFuncionarioAsync()
    {
        var descricoes = new[] { "professor", "zelador", "analista", "terceirizado" };
        var ids = new List<int>();

        var tipoFuncionarioDao = serviceProvider.GetRequiredService<TipoFuncionarioDAO>();

        foreach (var desc in descricoes)
        {
            var tipo = new TipoFuncionario { Descricao = desc };
            var id = await tipoFuncionarioDao.InsertAsync(tipo);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<List<int>> SeedSetoresAsync()
    {
        var nomes = new[] { "Gestao", "Suporte", "Logistica", "Tecnologia", "Contabilidade" };
        var ids = new List<int>();

        var setorDao = serviceProvider.GetRequiredService<SetorDAO>();

        foreach (var nome in nomes)
        {
            var sigla = string.Concat(nome.Split(' ').Select(w => w[0])); // [..Math.Min(10, nome.Length)];
            var setor = new Setor { Nome = nome, Sigla = sigla };
            var id = await setorDao.InsertAsync(setor);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<List<int>> SeedPessoasAsync(int total = 12)
    {
        var ids = new List<int>();

        var pessoaDao = serviceProvider.GetRequiredService<PessoaDAO>();

        for (var i = 0; i < total; i++)
        {
            var pessoa = new Pessoa
            {
                Nome = _faker.Name.FullName(),
                Cpf = _faker.Random.ReplaceNumbers("###########"),
                MatriculaSiape = $"S{_faker.Random.Int(5000, 99999)}",
                Email = _faker.Internet.Email(),
                Telefone = _faker.Phone.PhoneNumber() //[..20]
            };

            var id = await pessoaDao.InsertAsync(pessoa);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<List<int>> SeedFuncionariosAsync(
        List<int> pessoasIds,
        List<int> tiposIds,
        List<int> setoresIds,
        int total = 10
    )
    {
        var funcionariosIds = new List<int>();
        var funcionarioDao = serviceProvider.GetRequiredService<FuncionarioDAO>();
        var escolhidos = pessoasIds.OrderBy(_ => _random.Next()).Take(total).ToList();

        foreach (var pessoaId in escolhidos)
        {
            var funcionario = new Funcionario
            {
                PessoaId = pessoaId,
                TipoFuncionarioId = tiposIds[_random.Next(tiposIds.Count)],
                SetorId = setoresIds[_random.Next(setoresIds.Count)],
                DataAdmissao =
                    DateOnly.FromDateTime(_faker.Date.Between(DateTime.Now.AddYears(-3), DateTime.Now.AddDays(-15)))
            };

            var id = await funcionarioDao.InsertAsync(funcionario);
            funcionariosIds.Add(id);
        }

        return funcionariosIds;
    }

    private async Task<List<int>> SeedTiposAreaAsync()
    {
        var descricoes = new[] { "Administrativo", "Academico", "Externa" };
        var ids = new List<int>();

        var tipoAreaDao = serviceProvider.GetRequiredService<TipoAreaCampusDAO>();

        foreach (var desc in descricoes)
        {
            var tipo = new TipoAreaCampus { Descricao = desc };
            var id = await tipoAreaDao.InsertAsync(tipo);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<List<int>> SeedAreasAsync(List<int> tipoIds, int total = 6)
    {
        var areasIds = new List<int>();
        var areaDao = serviceProvider.GetRequiredService<AreaCampusDAO>();
        var blocos = new[] { "Alpha", "Beta", "Gamma", "Delta", "Sigma" };

        for (var i = 0; i < total; i++)
        {
            var area = new AreaCampus
            {
                Descricao = _faker.Address.StreetName(),
                Bloco = blocos[_random.Next(blocos.Length)],
                TipoAreaId = tipoIds[_random.Next(tipoIds.Count)]
            };

            var id = await areaDao.InsertAsync(area);
            areasIds.Add(id);
        }

        return areasIds;
    }

    private async Task<List<int>> SeedEquipesAsync(int total = 5)
    {
        var nomes = new[] { "Equipe Alfa", "Equipe Bravo", "Equipe Delta", "Equipe Eco", "Equipe Fox" };
        var turnos = new[] { "Manha", "Tarde", "Noite" };
        var equipesIds = new List<int>();

        var equipeDao = serviceProvider.GetRequiredService<EquipeManutencaoDAO>();
        foreach (var nome in nomes.OrderBy(_ => _random.Next()).Take(total))
        {
            var equipe = new EquipeManutencao
            {
                Nome = nome,
                Turno = turnos[_random.Next(turnos.Length)]
            };
            equipesIds.Add(await equipeDao.InsertAsync(equipe));
        }

        return equipesIds;
    }

    private async Task<List<int>> SeedEquipeMembrosAsync(List<int> equipesIds, List<int> funcionariosIds)
    {
        var membrosIds = new List<int>();
        var membroDao = serviceProvider.GetRequiredService<EquipeMembroDAO>();
        var funcoes = new[] { "Tecnico", "Apoio", "Coordenador" };

        foreach (var equipeId in equipesIds)
        {
            var quantidade = _random.Next(3, Math.Min(5, funcionariosIds.Count) + 1);
            foreach (var funcionarioId in funcionariosIds.OrderBy(_ => _random.Next()).Take(quantidade))
            {
                var inicio = _faker.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now.AddDays(-10));
                DateOnly? fim = _random.NextDouble() < 0.8
                    ? null
                    : DateOnly.FromDateTime(_faker.Date.Between(inicio, DateTime.Now));

                membrosIds.Add(await membroDao.InsertAsync(new EquipeMembro
                {
                    EquipeId = equipeId,
                    FuncionarioId = funcionarioId,
                    DataInicio = DateOnly.FromDateTime(inicio),
                    DataFim = fim,
                    Funcao = funcoes[_random.Next(funcoes.Length)]
                }));
            }
        }

        return membrosIds;
    }

    private async Task<List<int>> SeedCategoriasAsync()
    {
        var nomes = new[] { "Materiais Gerais", "Eletrica", "Hidraulica", "TI", "Ferramentas" };
        var categoriaDao = serviceProvider.GetRequiredService<CategoriaMaterialDAO>();
        var categoriasIds = new List<int>();

        foreach (var nome in nomes)
        {
            categoriasIds.Add(await categoriaDao.InsertAsync(new CategoriaMaterial { Nome = nome }));
        }

        return categoriasIds;
    }

    private async Task<List<int>> SeedUnidadesAsync()
    {
        var unidades = new (string Sigla, string Descricao)[]
        {
            ("UN", "Unidade"),
            ("KG", "Quilograma"),
            ("M", "Metro"),
            ("L", "Litro")
        };

        var ids = new List<int>();
        var unidadeDao = serviceProvider.GetRequiredService<UnidadeMedidaDAO>();

        foreach (var (sigla, descricao) in unidades)
        {
            var unidade = new UnidadeMedida
            {
                Sigla = sigla,
                Descricao = descricao
            };
            var id = await unidadeDao.InsertAsync(unidade);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<List<int>> SeedFornecedoresAsync(int total = 6) // mudou de 5 para 6
    {
        var ids = new List<int>();
        var fornecedorDao = serviceProvider.GetRequiredService<FornecedorDAO>();

        for (var i = 0; i < total; i++)
        {
            var fornecedor = new Fornecedor
            {
                Nome = _faker.Company.CompanyName(),
                Cnpj = _faker.Company.Cnpj()
            };

            var id = await fornecedorDao.InsertAsync(fornecedor);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<List<int>> SeedMarcasAsync(int total = 6)
    {
        var ids = new List<int>();
        var marcaDao = serviceProvider.GetRequiredService<MarcaDAO>();

        for (var i = 0; i < total; i++)
        {
            var marca = new Marca
            {
                Nome = _faker.Company.CompanySuffix()
            };

            var id = await marcaDao.InsertAsync(marca);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<List<int>> SeedCoresAsync()
    {
        var nomes = new[] { "Vermelho", "Azul", "Preto", "Branco", "Cinza", "Verde" };
        var coresIds = new List<int>();
        var corDao = serviceProvider.GetRequiredService<CorDAO>();

        foreach (var nome in nomes)
        {
            var cor = new Cor { Nome = nome };
            var id = await corDao.InsertAsync(cor);
            coresIds.Add(id);
        }

        return coresIds;
    }

    private async Task<List<int>> SeedTamanhosAsync()
    {
        var descricoes = new[] { "PP", "P", "M", "G", "GG", "U", "EX" }; // adicionado "EX" como tamanho extra
        var ids = new List<int>();
        var tamanhoDao = serviceProvider.GetRequiredService<TamanhoDAO>();

        foreach (var descricao in descricoes)
        {
            var tamanho = new Tamanho { Descricao = descricao };
            var id = await tamanhoDao.InsertAsync(tamanho);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<List<int>> SeedProdutosAsync(
        List<int> categoriasIds,
        List<int> unidadesIds,
        List<int> marcasIds,
        int total = 8
    )
    {
        var ids = new List<int>();
        var produtoDao = serviceProvider.GetRequiredService<ProdutoDAO>();

        for (var i = 0; i < total; i++)
        {
            var produto = new Produto
            {
                Descricao = _faker.Commerce.ProductName(), // Bogus equivalente ao catch_phrase
                CategoriaId = categoriasIds[_random.Next(categoriasIds.Count)],
                UnidadeMedidaId = unidadesIds[_random.Next(unidadesIds.Count)],
                MarcaId = marcasIds[_random.Next(marcasIds.Count)]
            };

            var id = await produtoDao.InsertAsync(produto);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<List<int>> SeedVariacoesAsync(
        List<int> produtosIds,
        List<int> coresIds,
        List<int> tamanhosIds
    )
    {
        var ids = new List<int>();
        var combinacoesUsadas = new HashSet<(int ProdutoId, int CorId, int TamanhoId)>();
        var variacaoDao = serviceProvider.GetRequiredService<ProdutoVariacaoDAO>();

        foreach (var produtoId in produtosIds)
        {
            var coresEscolhidas = coresIds.OrderBy(_ => _random.Next()).Take(Math.Min(2, coresIds.Count)).ToList();
            var tamanhosEscolhidos =
                tamanhosIds.OrderBy(_ => _random.Next()).Take(Math.Min(2, tamanhosIds.Count)).ToList();

            foreach (var corId in coresEscolhidas)
            {
                var tamanhoId = tamanhosEscolhidos[_random.Next(tamanhosEscolhidos.Count)];

                if (combinacoesUsadas.Contains((produtoId, corId, tamanhoId)))
                    continue;

                combinacoesUsadas.Add((produtoId, corId, tamanhoId));

                var variacao = new ProdutoVariacao
                {
                    ProdutoId = produtoId,
                    CorId = corId,
                    TamanhoId = tamanhoId,
                    CodigoBarras = _faker.Commerce.Ean13(),
                    CodigoInterno = $"PRD-{_faker.Random.Int(1000, 9999)}"
                };

                var id = await variacaoDao.InsertAsync(variacao);
                ids.Add(id);
            }
        }

        return ids;
    }

    private async Task<List<int>>
        SeedLocaisAsync(List<int> funcionariosIds, int total = 6)
    {
        var ids = new List<int>();
        var localDao = serviceProvider.GetRequiredService<LocalEstoqueDAO>();

        for (var idx = 0; idx < total; idx++)
        {
            var local = new LocalEstoque
            {
                Descricao = $"Deposito {idx + 1}",
                ResponsavelId = funcionariosIds[_random.Next(funcionariosIds.Count)]
            };

            var id = await localDao.InsertAsync(local);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<Dictionary<string, int>> SeedTiposMovimentoAsync()
    {
        var tipos = new (string Descricao, char Sinal)[]
        {
            ("Entrada", '+'),
            ("Saida", '-'),
            ("Ajuste", '+')
        };

        var ids = new Dictionary<string, int>();
        var tipoMovimentoDao = serviceProvider.GetRequiredService<TipoMovimentoEstoqueDAO>();

        foreach (var (descricao, sinal) in tipos)
        {
            var tipo = new TipoMovimentoEstoque
            {
                Descricao = descricao,
                Sinal = sinal
            };

            var id = await tipoMovimentoDao.InsertAsync(tipo);
            ids[descricao.ToLower()] = id;
        }

        return ids;
    }

    private async Task<int> RegistrarMovimentoAsync(
        int produtoVariacaoId,
        int localId,
        decimal quantidade,
        int tipoMovimentoId,
        int? funcionarioId = null,
        int? osId = null,
        string? observacao = null
    )
    {
        var tipoMovimentoDao = serviceProvider.GetRequiredService<TipoMovimentoEstoqueDAO>();
        var estoqueDao = serviceProvider.GetRequiredService<EstoqueDAO>();
        var movimentoDao = serviceProvider.GetRequiredService<MovimentoEstoqueDAO>();

        var tipoMovimento = await tipoMovimentoDao.FindByIdAsync(tipoMovimentoId);
        if (tipoMovimento == null)
            throw new ArgumentException("Tipo de movimento inválido");

        var estoqueAtual = await estoqueDao.FindAsync(produtoVariacaoId, localId);
        if (estoqueAtual == null)
        {
            estoqueAtual = new Estoque
            {
                ProdutoVariacaoId = produtoVariacaoId,
                LocalEstoqueId = localId,
                Quantidade = 0m,
                PontoReposicao = 5m
            };
            await estoqueDao.UpsertAsync(estoqueAtual);
        }

        if (tipoMovimento.Sinal == '-' && estoqueAtual.Quantidade < quantidade)
        {
            quantidade = estoqueAtual.Quantidade!.Value;
        }

        var delta = tipoMovimento.Sinal == '+' ? quantidade : -quantidade;

        var movimento = new MovimentoEstoque
        {
            ProdutoVariacaoId = produtoVariacaoId,
            LocalEstoqueId = localId,
            TipoMovimentoId = tipoMovimentoId,
            Quantidade = quantidade,
            DataHora = DateTime.Now,
            FuncionarioId = funcionarioId,
            OrdemServicoId = osId,
            Observacao = observacao
        };

        var novoMovimentoId = await movimentoDao.InsertAsync(movimento);
        await estoqueDao.AjustarQuantidadeAsync(produtoVariacaoId, localId, delta);

        return novoMovimentoId;
    }

    private async Task<List<(int ProdutoVariacaoId, int LocalId)>> SeedEstoqueAsync(
        List<int> variacoesIds,
        List<int> locaisIds,
        Dictionary<string, int> tipoMovimentos,
        List<int> funcionariosIds,
        List<int>? osIds = null
    )
    {
        var estoquePairs = new List<(int, int)>();
        var estoqueDao = serviceProvider.GetRequiredService<EstoqueDAO>();
        var movimentoDao = serviceProvider.GetRequiredService<MovimentoEstoqueDAO>();

        for (var idx = 0; idx < variacoesIds.Count; idx++)
        {
            var variacaoId = variacoesIds[idx];
            var localId = locaisIds[_random.Next(locaisIds.Count)];
            decimal pontoReposicao = _random.Next(5, 11); // 5 a 10

            // Upsert estoque inicial
            await estoqueDao.UpsertAsync(new Estoque
            {
                ProdutoVariacaoId = variacaoId,
                LocalEstoqueId = localId,
                Quantidade = 0m,
                PontoReposicao = pontoReposicao
            });

            var quantidade = Math.Round((decimal)(_random.NextDouble() * 15 + 5), 2); // 5 a 20 com 2 casas

            // Registro de entrada
            var movId = await RegistrarMovimentoAsync(
                produtoVariacaoId: variacaoId,
                localId: localId,
                quantidade: quantidade,
                tipoMovimentoId: tipoMovimentos["entrada"],
                funcionarioId: funcionariosIds[_random.Next(funcionariosIds.Count)],
                osId: osIds != null ? osIds[_random.Next(osIds.Count)] : null,
                observacao: "Carga inicial"
            );

            if (osIds != null)
            {
                await movimentoDao.AtualizarDataEOsAsync(
                    movId,
                    new DateTime(2025, 10, _random.Next(1, 31), _random.Next(7, 19), _random.Next(0, 60), 0),
                    osIds[_random.Next(osIds.Count)]
                );
            }

            // Movimentos de saída para os primeiros 3 produtos
            if (idx < 3)
            {
                var qtdSaida = Math.Min(quantidade, Math.Max(1m, quantidade - pontoReposicao + 1m));

                var movSaida = await RegistrarMovimentoAsync(
                    produtoVariacaoId: variacaoId,
                    localId: localId,
                    quantidade: qtdSaida,
                    tipoMovimentoId: tipoMovimentos["saida"],
                    funcionarioId: funcionariosIds[_random.Next(funcionariosIds.Count)],
                    osId: osIds != null ? osIds[_random.Next(osIds.Count)] : null,
                    observacao: "Reducao proposital para teste"
                );

                if (osIds != null)
                {
                    await movimentoDao.AtualizarDataEOsAsync(
                        movSaida,
                        new DateTime(2025, 10, _random.Next(1, 31), _random.Next(7, 19), _random.Next(0, 60), 0),
                        osIds[_random.Next(osIds.Count)]
                    );
                }
            }

            estoquePairs.Add((variacaoId, localId));
        }

        return estoquePairs;
    }

    private async Task SeedMovimentosExtrasAsync(
        List<(int ProdutoVariacaoId, int LocalId)> estoquePairs,
        Dictionary<string, int> tipoMovimentos,
        List<int> funcionariosIds,
        List<int>? osIds = null
    )
    {
        var quantidadeMovimentos = Math.Max(1, estoquePairs.Count / 3);
        var selecionados = estoquePairs.OrderBy(_ => _random.Next()).Take(quantidadeMovimentos);

        foreach (var (variacaoId, localId) in selecionados)
        {
            var quantidade = Math.Round((decimal)(_random.NextDouble() * 4 + 1), 2); // 1 a 5 com 2 casas
            var tipoMovimentoId = _random.NextDouble() < 0.5
                ? tipoMovimentos["saida"]
                : tipoMovimentos["ajuste"];

            var movId = await RegistrarMovimentoAsync(
                produtoVariacaoId: variacaoId,
                localId: localId,
                quantidade: quantidade,
                tipoMovimentoId: tipoMovimentoId,
                funcionarioId: funcionariosIds[_random.Next(funcionariosIds.Count)],
                osId: osIds != null ? osIds[_random.Next(osIds.Count)] : null,
                observacao: "Movimento automatizado"
            );

            if (osIds != null)
            {
                var movimentoDao = serviceProvider.GetRequiredService<MovimentoEstoqueDAO>();
                await movimentoDao.AtualizarDataEOsAsync(
                    movId,
                    new DateTime(2025, 10, _random.Next(1, 31), _random.Next(7, 19), _random.Next(0, 60), 0),
                    osIds[_random.Next(osIds.Count)]
                );
            }
        }
    }

    private async Task AtualizarStatusOsAsync(
        int osId,
        int novoStatusId,
        int funcionarioId,
        string descricao,
        DateTime? inicioAtendimento = null,
        DateTime? fimAtendimento = null
    )
    {
        var osDao = serviceProvider.GetRequiredService<OrdemServicoDAO>();
        var andamentoDao = serviceProvider.GetRequiredService<AndamentoOrdemServicoDAO>();

        var osAtual = await osDao.FindByIdAsync(osId);
        var statusAnterior = osAtual?.StatusOrdemServicoId;

        await osDao.UpdateStatusAsync(osId, novoStatusId);

        var andamento = new AndamentoOrdemServico
        {
            OrdemServicoId = osId,
            DataHora = DateTime.Now,
            StatusAnteriorId = statusAnterior,
            StatusNovoId = novoStatusId,
            FuncionarioId = funcionarioId,
            Descricao = descricao,
            InicioAtendimento = inicioAtendimento,
            FimAtendimento = fimAtendimento
        };

        await andamentoDao.InsertAsync(andamento);
    }

    private async Task<List<int>> SeedTiposOsAsync()
    {
        var descricoes = new[] { "Manutencao Eletrica", "Manutencao Hidraulica", "Infraestrutura", "TI" };
        var ids = new List<int>();
        var tipoOsDao = serviceProvider.GetRequiredService<TipoOrdemServicoDAO>();

        foreach (var desc in descricoes)
        {
            var tipo = new TipoOrdemServico { Descricao = desc };
            var id = await tipoOsDao.InsertAsync(tipo);
            ids.Add(id);
        }

        return ids;
    }

    private async Task<Dictionary<string, int>> SeedStatusOsAsync()
    {
        var descricoes = new[] { "Aberta", "Em Atendimento", "Aguardando Material", "Concluida", "Cancelada" };
        var statusDao = serviceProvider.GetRequiredService<StatusOrdemServicoDAO>();
        var ids = new Dictionary<string, int>();

        foreach (var desc in descricoes)
        {
            var status = new StatusOrdemServico { Descricao = desc };
            var id = await statusDao.InsertAsync(status);
            ids[desc.ToLower().Replace(" ", "_")] = id;
        }

        return ids;
    }

    private async Task<List<int>> SeedOrdensServicoAsync(
        List<int> solicitantesIds,
        List<int> areasIds,
        List<int> tiposOsIds,
        List<int> equipesIds,
        List<int> lideresIds,
        List<int> variacoesIds,
        Dictionary<string, int> statusMap,
        int total = 6 // aumentei de 5 para 6 ordens
    )
    {
        var osIds = new List<int>();
        var baseData = new DateTime(2025, 10, 10, 9, 0, 0);

        var osDao = serviceProvider.GetRequiredService<OrdemServicoDAO>();
        var itemDao = serviceProvider.GetRequiredService<ItemOrdemServicoDAO>();
        var andamentoDao = serviceProvider.GetRequiredService<AndamentoOrdemServicoDAO>();

        for (var idx = 0; idx < total; idx++)
        {
            var itens = new List<(int ProdutoVariacaoId, decimal QuantidadePrevista, decimal? QuantidadeUsada)>();
            var abertura = baseData.AddDays(_random.Next(-5, 6)).AddHours(_random.Next(0, 7));
            var previsao = DateOnly.FromDateTime(abertura.Date.AddDays(_random.Next(2, 11)));

            // Itens da OS
            foreach (var variacaoId in variacoesIds.OrderBy(_ => _random.Next()).Take(_random.Next(1, 4)))
            {
                var quantidadePrevista = Math.Round((decimal)(_random.NextDouble() * 3 + 1), 1); // 1.0 a 4.0
                itens.Add((variacaoId, quantidadePrevista, null));
            }

            var ordemServico = new OrdemServico
            {
                NumeroSequencial = $"OS-{idx + 1:0000}",
                SolicitanteId = solicitantesIds[_random.Next(solicitantesIds.Count)],
                AreaCampusId = areasIds[_random.Next(areasIds.Count)],
                TipoOrdemServicoId = tiposOsIds[_random.Next(tiposOsIds.Count)],
                EquipeId = equipesIds[_random.Next(equipesIds.Count)],
                LiderId = lideresIds[_random.Next(lideresIds.Count)],
                StatusOrdemServicoId = statusMap["aberta"],
                Prioridade = _random.Next(1, 6), // 1 a 5
                DataAbertura = abertura,
                DataPrevista = previsao,
                DescricaoProblema = _faker.Lorem.Sentence(12)
            };

            var osId = await osDao.InsertAsync(ordemServico);

            // Inserir itens
            foreach (var item in itens)
            {
                await itemDao.InsertAsync(new ItemOrdemServico
                {
                    OrdemServicoId = osId,
                    ProdutoVariacaoId = item.ProdutoVariacaoId,
                    QuantidadePrevista = item.QuantidadePrevista,
                    QuantidadeUsada = item.QuantidadeUsada
                });
            }

            // Registrar andamento inicial
            await andamentoDao.InsertAsync(new AndamentoOrdemServico
            {
                OrdemServicoId = osId,
                DataHora = abertura,
                StatusAnteriorId = null,
                StatusNovoId = statusMap["aberta"],
                FuncionarioId = lideresIds[_random.Next(lideresIds.Count)],
                Descricao = "Abertura da OS"
            });

            osIds.Add(osId);
        }

        return osIds;
    }

    private async Task SeedAndamentosAsync(
        List<int> ordensIds,
        Dictionary<string, int> statusMap,
        List<int> lideresIds
    )
    {
        string[] cicloStatus = { "em_atendimento", "aguardando_material", "aberta", "concluida", "concluida" };

        for (var idx = 0; idx < ordensIds.Count; idx++)
        {
            var osId = ordensIds[idx];
            var alvo = cicloStatus[idx % cicloStatus.Length];
            var lider = lideresIds[_random.Next(lideresIds.Count)];

            if (alvo != "aberta")
            {
                await AtualizarStatusOsAsync(
                    osId: osId,
                    novoStatusId: statusMap[alvo],
                    funcionarioId: lider,
                    descricao: "Atualizacao automatica",
                    inicioAtendimento: DateTime.Now.AddDays(-1),
                    fimAtendimento: alvo == "concluida" ? DateTime.Now : null
                );
            }

            if (idx == 0)
            {
                // Simula espera por material
                await AtualizarStatusOsAsync(
                    osId: osId,
                    novoStatusId: statusMap["aguardando_material"],
                    funcionarioId: lider,
                    descricao: "Aguardando material para compra",
                    inicioAtendimento: DateTime.Now.AddHours(-3),
                    fimAtendimento: null
                );

                // Retoma atendimento
                await AtualizarStatusOsAsync(
                    osId: osId,
                    novoStatusId: statusMap["em_atendimento"],
                    funcionarioId: lider,
                    descricao: "Retomado atendimento",
                    inicioAtendimento: DateTime.Now.AddHours(-1),
                    fimAtendimento: null
                );
            }
        }
    }
}