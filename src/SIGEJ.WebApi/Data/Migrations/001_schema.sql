CREATE TABLE IF NOT EXISTS pessoa
(
    id              SERIAL PRIMARY KEY,
    nome            VARCHAR(100) NOT NULL,
    cpf             VARCHAR(11) UNIQUE,
    matricula_siape VARCHAR(20),
    email           VARCHAR(100),
    telefone        VARCHAR(20),
    ativo           BOOLEAN DEFAULT true
);

CREATE TABLE IF NOT EXISTS tipo_funcionario
(
    id        SERIAL PRIMARY KEY,
    descricao VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS setor
(
    id    SERIAL PRIMARY KEY,
    nome  VARCHAR(80) NOT NULL,
    sigla VARCHAR(10)
);

CREATE TABLE IF NOT EXISTS funcionario
(
    id                  SERIAL PRIMARY KEY,
    pessoa_id           INT REFERENCES pessoa (id),
    tipo_funcionario_id INT REFERENCES tipo_funcionario (id),
    setor_id            INT REFERENCES setor (id),
    data_admissao       DATE,
    data_demissao       DATE
);

CREATE TABLE IF NOT EXISTS tipo_area_campus
(
    id        SERIAL PRIMARY KEY,
    descricao VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS area_campus
(
    id           SERIAL PRIMARY KEY,
    tipo_area_id INT REFERENCES tipo_area_campus (id),
    descricao    VARCHAR(100) NOT NULL,
    bloco        VARCHAR(10)
);

CREATE TABLE IF NOT EXISTS equipe_manutencao
(
    id    SERIAL PRIMARY KEY,
    nome  VARCHAR(80) NOT NULL,
    turno VARCHAR(20)
);

CREATE TABLE IF NOT EXISTS equipe_membro
(
    id             SERIAL PRIMARY KEY,
    equipe_id      INT REFERENCES equipe_manutencao (id),
    funcionario_id INT REFERENCES funcionario (id),
    data_inicio    DATE NOT NULL,
    data_fim       DATE,
    funcao         VARCHAR(30)
);

CREATE TABLE IF NOT EXISTS categoria_material
(
    id   SERIAL PRIMARY KEY,
    nome VARCHAR(60) NOT NULL
);

CREATE TABLE IF NOT EXISTS unidade_medida
(
    id        SERIAL PRIMARY KEY,
    sigla     VARCHAR(10) NOT NULL,
    descricao VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS fornecedor
(
    id   SERIAL PRIMARY KEY,
    nome VARCHAR(100),
    cnpj VARCHAR(18)
);

CREATE TABLE IF NOT EXISTS marca
(
    id   SERIAL PRIMARY KEY,
    nome VARCHAR(80)
);

CREATE TABLE IF NOT EXISTS produto
(
    id                SERIAL PRIMARY KEY,
    descricao         TEXT NOT NULL,
    categoria_id      INT REFERENCES categoria_material (id),
    unidade_medida_id INT REFERENCES unidade_medida (id),
    marca_id          INT REFERENCES marca (id)
);

CREATE TABLE IF NOT EXISTS cor
(
    id   SERIAL PRIMARY KEY,
    nome VARCHAR(30)
);

CREATE TABLE IF NOT EXISTS tamanho
(
    id        SERIAL PRIMARY KEY,
    descricao VARCHAR(30)
);

CREATE TABLE IF NOT EXISTS produto_variacao
(
    id             SERIAL PRIMARY KEY,
    produto_id     INT REFERENCES produto (id),
    cor_id         INT REFERENCES cor (id),
    tamanho_id     INT REFERENCES tamanho (id),
    codigo_barras  VARCHAR(50) UNIQUE,
    codigo_interno VARCHAR(30)
);

CREATE TABLE IF NOT EXISTS local_estoque
(
    id             SERIAL PRIMARY KEY,
    descricao      VARCHAR(100),
    responsavel_id INT REFERENCES funcionario (id)
);

CREATE TABLE IF NOT EXISTS estoque
(
    produto_variacao_id INT REFERENCES produto_variacao (id),
    local_estoque_id    INT REFERENCES local_estoque (id),
    quantidade          DECIMAL(10, 3) DEFAULT 0,
    ponto_reposicao     DECIMAL(10, 3) DEFAULT 0,
    PRIMARY KEY (produto_variacao_id, local_estoque_id)
);

CREATE TABLE IF NOT EXISTS tipo_movimento_estoque
(
    id        SERIAL PRIMARY KEY,
    descricao VARCHAR(50),
    sinal     CHAR(1) CHECK (sinal IN ('+', '-'))
);

CREATE TABLE IF NOT EXISTS movimento_estoque
(
    id                  SERIAL PRIMARY KEY,
    produto_variacao_id INT REFERENCES produto_variacao (id),
    local_estoque_id    INT REFERENCES local_estoque (id),
    tipo_movimento_id   INT REFERENCES tipo_movimento_estoque (id),
    quantidade          DECIMAL(10, 3) NOT NULL,
    data_hora           TIMESTAMP DEFAULT NOW(),
    funcionario_id      INT REFERENCES funcionario (id),
    ordem_servico_id    INT,
    observacao          TEXT
);

CREATE TABLE IF NOT EXISTS tipo_ordem_servico
(
    id        SERIAL PRIMARY KEY,
    descricao VARCHAR(80)
);

CREATE TABLE IF NOT EXISTS status_ordem_servico
(
    id        SERIAL PRIMARY KEY,
    descricao VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS ordem_servico
(
    id                 SERIAL PRIMARY KEY,
    numero_sequencial  VARCHAR(20) UNIQUE,
    solicitante_id     INT REFERENCES pessoa (id),
    area_campus_id     INT REFERENCES area_campus (id),
    tipo_os_id         INT REFERENCES tipo_ordem_servico (id),
    equipe_id          INT REFERENCES equipe_manutencao (id),
    lider_id           INT REFERENCES funcionario (id),
    status_id          INT REFERENCES status_ordem_servico (id) DEFAULT 1,
    prioridade         INT CHECK (prioridade IN (1, 2, 3, 4, 5)),
    data_abertura      TIMESTAMP                                DEFAULT NOW(),
    data_prevista      DATE,
    descricao_problema TEXT
);

CREATE TABLE IF NOT EXISTS item_ordem_servico
(
    id                  SERIAL PRIMARY KEY,
    os_id               INT REFERENCES ordem_servico (id) ON DELETE CASCADE,
    produto_variacao_id INT REFERENCES produto_variacao (id),
    quantidade_prevista DECIMAL(10, 3),
    quantidade_usada    DECIMAL(10, 3)
);

CREATE TABLE IF NOT EXISTS andamento_ordem_servico
(
    id                 SERIAL PRIMARY KEY,
    os_id              INT REFERENCES ordem_servico (id) ON DELETE CASCADE,
    data_hora          TIMESTAMP DEFAULT NOW(),
    status_anterior_id INT REFERENCES status_ordem_servico (id),
    status_novo_id     INT REFERENCES status_ordem_servico (id),
    funcionario_id     INT REFERENCES funcionario (id),
    descricao          TEXT,
    inicio_atendimento TIMESTAMP,
    fim_atendimento    TIMESTAMP
);