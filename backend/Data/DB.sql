-- Active: 1772829380216@@localhost@3306@estoquesa
CREATE DATABASE IF NOT EXISTS EstoqueSA;

USE EstoqueSA;

-- ==================== USUARIOS ====================

CREATE TABLE IF NOT EXISTS Perfil (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Usuario (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    login VARCHAR(255) NOT NULL UNIQUE,
    senhaHash VARCHAR(255) NOT NULL,
    ativo BOOLEAN NOT NULL DEFAULT true,
    criadoEm DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    perfilId INT NOT NULL,
    CONSTRAINT Usuario_perfilId_fkey FOREIGN KEY (perfilId) REFERENCES Perfil (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS EventoUsuario (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    usuarioId INT,
    tipoEvento VARCHAR(255) NOT NULL,
    descricao TEXT,
    ip VARCHAR(45),
    dataEvento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT EventoUsuario_usuarioId_fkey FOREIGN KEY (usuarioId) REFERENCES Usuario (id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ==================== ESTOQUE ====================

CREATE TABLE IF NOT EXISTS Produto (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    descricao TEXT,
    preco DECIMAL(10,2)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Estoque (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    produtoId INT NOT NULL,
    quantidade INT NOT NULL DEFAULT 0,
    estoqueMinimo INT NOT NULL DEFAULT 0,
    unidadeMedida VARCHAR(50),
    CONSTRAINT Estoque_produtoId_fkey FOREIGN KEY (produtoId) REFERENCES Produto (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Movimentacao (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    estoqueId INT NOT NULL,
    usuarioId INT,
    tipo VARCHAR(50) NOT NULL,
    quantidade INT NOT NULL,
    dataMovimento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT Movimentacao_estoqueId_fkey FOREIGN KEY (estoqueId) REFERENCES Estoque (id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT Movimentacao_usuarioId_fkey FOREIGN KEY (usuarioId) REFERENCES Usuario (id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ==================== PRODUCAO ====================

CREATE TABLE IF NOT EXISTS EtapaProducao (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nomeEtapa VARCHAR(100) NOT NULL,
    descricao VARCHAR(500),
    ordem INT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS OrdemProducao (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    numeroOp INT NOT NULL UNIQUE,
    produtoId INT NOT NULL,
    quantidade INT NOT NULL,
    status VARCHAR(50),
    CONSTRAINT OrdemProducao_produtoId_fkey FOREIGN KEY (produtoId) REFERENCES Produto (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS EtapaOP (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    opId INT NOT NULL,
    etapaId INT NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Pendente',
    dataInicio DATETIME NULL,
    dataFim DATETIME NULL,
    CONSTRAINT EtapaOP_opId_fkey FOREIGN KEY (opId) REFERENCES OrdemProducao (id) ON DELETE CASCADE,
    CONSTRAINT EtapaOP_etapaId_fkey FOREIGN KEY (etapaId) REFERENCES EtapaProducao (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ==================== EQUIPAMENTOS ====================

CREATE TABLE IF NOT EXISTS Equipamento (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    setor VARCHAR(100),
    status VARCHAR(50)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Manutencao (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    equipamentoId INT NOT NULL,
    tipo VARCHAR(100) NOT NULL,
    descricao TEXT,
    dataManutencao DATETIME,
    responsavelId INT,
    CONSTRAINT Manutencao_equipamentoId_fkey FOREIGN KEY (equipamentoId) REFERENCES Equipamento (id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT Manutencao_responsavelId_fkey FOREIGN KEY (responsavelId) REFERENCES Usuario (id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Indicador (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    equipamentoId INT NOT NULL,
    temperatura DOUBLE,
    vibracao DOUBLE,
    horasUso INT,
    dataRegistro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT Indicador_equipamentoId_fkey FOREIGN KEY (equipamentoId) REFERENCES Equipamento (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Alerta (
    id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    equipamentoId INT NOT NULL,
    tipo VARCHAR(100) NOT NULL,
    descricao TEXT,
    dataAlerta DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT Alerta_equipamentoId_fkey FOREIGN KEY (equipamentoId) REFERENCES Equipamento (id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


