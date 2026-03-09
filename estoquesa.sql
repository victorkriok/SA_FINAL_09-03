-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Tempo de geração: 09/03/2026 às 02:29
-- Versão do servidor: 10.4.32-MariaDB
-- Versão do PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Banco de dados: `estoquesa`
--

-- --------------------------------------------------------

--
-- Estrutura para tabela `alerta`
--

CREATE TABLE `alerta` (
  `id_alerta` int(11) NOT NULL,
  `id_sensor` int(11) DEFAULT NULL,
  `tipo_alerta` varchar(50) DEFAULT NULL,
  `valor_detectado` decimal(10,2) DEFAULT NULL,
  `limite_configurado` decimal(10,2) DEFAULT NULL,
  `mensagem` text DEFAULT NULL,
  `data_alerta` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `equipamento`
--

CREATE TABLE `equipamento` (
  `id` int(11) NOT NULL,
  `nome` varchar(255) NOT NULL,
  `setor` varchar(100) DEFAULT NULL,
  `status` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `estoque`
--

CREATE TABLE `estoque` (
  `id` int(11) NOT NULL,
  `produtoId` int(11) NOT NULL,
  `quantidade` int(11) NOT NULL DEFAULT 0,
  `estoqueMinimo` int(11) NOT NULL DEFAULT 0,
  `unidadeMedida` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


-- --------------------------------------------------------

--
-- Estrutura para tabela `etapaop`
--

CREATE TABLE `etapaop` (
  `id` int(11) NOT NULL,
  `opId` int(11) NOT NULL,
  `etapaId` int(11) NOT NULL,
  `status` varchar(50) NOT NULL DEFAULT 'Pendente',
  `dataInicio` datetime DEFAULT NULL,
  `dataFim` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `etapaproducao`
--

CREATE TABLE `etapaproducao` (
  `id` int(11) NOT NULL,
  `nomeEtapa` varchar(100) NOT NULL,
  `descricao` varchar(500) DEFAULT NULL,
  `ordem` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `eventousuario`
--

CREATE TABLE `eventousuario` (
  `id` int(11) NOT NULL,
  `usuarioId` int(11) DEFAULT NULL,
  `tipoEvento` varchar(255) NOT NULL,
  `descricao` text DEFAULT NULL,
  `ip` varchar(45) DEFAULT NULL,
  `dataEvento` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `indicador`
--

CREATE TABLE `indicador` (
  `id` int(11) NOT NULL,
  `equipamentoId` int(11) NOT NULL,
  `temperatura` double DEFAULT NULL,
  `vibracao` double DEFAULT NULL,
  `horasUso` int(11) DEFAULT NULL,
  `dataRegistro` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `leituras`
--

CREATE TABLE `leituras` (
  `id_leitura` int(11) NOT NULL,
  `id_sensor` int(11) DEFAULT NULL,
  `tipo_leitura` varchar(50) DEFAULT NULL,
  `valor` decimal(10,2) DEFAULT NULL,
  `unidade` varchar(20) DEFAULT NULL,
  `descricao` text DEFAULT NULL,
  `data_leitura` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `manutencao`
--

CREATE TABLE `manutencao` (
  `id` int(11) NOT NULL,
  `equipamentoId` int(11) NOT NULL,
  `tipo` varchar(100) NOT NULL,
  `descricao` text DEFAULT NULL,
  `dataManutencao` datetime DEFAULT NULL,
  `responsavelId` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `movimentacao`
--

CREATE TABLE `movimentacao` (
  `id` int(11) NOT NULL,
  `estoqueId` int(11) NOT NULL,
  `usuarioId` int(11) DEFAULT NULL,
  `tipo` varchar(50) NOT NULL,
  `quantidade` int(11) NOT NULL,
  `dataMovimento` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `ordemproducao`
--

CREATE TABLE `ordemproducao` (
  `id` int(11) NOT NULL,
  `numeroOp` int(11) NOT NULL,
  `produtoId` int(11) NOT NULL,
  `quantidade` int(11) NOT NULL,
  `status` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


-- --------------------------------------------------------

--
-- Estrutura para tabela `perfil`
--

CREATE TABLE `perfil` (
  `id` int(11) NOT NULL,
  `nome` varchar(255) NOT NULL,
  `descricao` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `perfil`
--

INSERT INTO `perfil` (`id`, `nome`, `descricao`) VALUES
(1, 'Admin', 'Acesso total ao sistema'),
(2, 'SupervisorProducao', 'Gerencia ordens de produção'),
(3, 'TecnicoManutencao', 'Gerencia manutenção de equipamentos'),
(4, 'Almoxarife', 'Gerencia estoque');

-- --------------------------------------------------------

--
-- Estrutura para tabela `produto`
--

CREATE TABLE `produto` (
  `id` int(11) NOT NULL,
  `nome` varchar(255) NOT NULL,
  `descricao` text DEFAULT NULL,
  `preco` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estrutura para tabela `sensores`
--

CREATE TABLE `sensores` (
  `id_sensor` int(11) NOT NULL,
  `nome_equipamento` varchar(100) DEFAULT NULL,
  `tipo_sensor` varchar(50) DEFAULT NULL,
  `localizacao` varchar(100) DEFAULT NULL,
  `status_ativo` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `sensores`
--

INSERT INTO `sensores` (`id_sensor`, `nome_equipamento`, `tipo_sensor`, `localizacao`, `status_ativo`) VALUES
(1, 'sensorTeste', 'Teste', '40028922', 1),
(2, 'teste2', 'Teste', '40028922', 1),
(3, 'teste3', 'teste', '40028922', 1);

-- --------------------------------------------------------

--
-- Estrutura para tabela `usuario`
--

CREATE TABLE `usuario` (
  `id` int(11) NOT NULL,
  `nome` varchar(255) NOT NULL,
  `login` varchar(255) NOT NULL,
  `senhaHash` varchar(255) NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT 1,
  `criadoEm` datetime NOT NULL DEFAULT current_timestamp(),
  `perfilId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `usuario`
--

INSERT INTO `usuario` (`id`, `nome`, `login`, `senhaHash`, `ativo`, `criadoEm`, `perfilId`) VALUES
(1, 'Admin', 'admin', '$2a$11$9poz4QMdyqLaLd.2UPn9D.nNLmx3yJKHWpopspEtgnn4tjpBYEK/e', 1, '2026-03-08 14:55:54', 1);

--
-- Índices para tabelas despejadas
--

--
-- Índices de tabela `alerta`
--
ALTER TABLE `alerta`
  ADD PRIMARY KEY (`id_alerta`),
  ADD KEY `id_sensor` (`id_sensor`);

--
-- Índices de tabela `equipamento`
--
ALTER TABLE `equipamento`
  ADD PRIMARY KEY (`id`);

--
-- Índices de tabela `estoque`
--
ALTER TABLE `estoque`
  ADD PRIMARY KEY (`id`),
  ADD KEY `Estoque_produtoId_fkey` (`produtoId`);

--
-- Índices de tabela `etapaop`
--
ALTER TABLE `etapaop`
  ADD PRIMARY KEY (`id`),
  ADD KEY `EtapaOP_opId_fkey` (`opId`),
  ADD KEY `EtapaOP_etapaId_fkey` (`etapaId`);

--
-- Índices de tabela `etapaproducao`
--
ALTER TABLE `etapaproducao`
  ADD PRIMARY KEY (`id`);

--
-- Índices de tabela `eventousuario`
--
ALTER TABLE `eventousuario`
  ADD PRIMARY KEY (`id`),
  ADD KEY `EventoUsuario_usuarioId_fkey` (`usuarioId`);

--
-- Índices de tabela `indicador`
--
ALTER TABLE `indicador`
  ADD PRIMARY KEY (`id`),
  ADD KEY `Indicador_equipamentoId_fkey` (`equipamentoId`);

--
-- Índices de tabela `leituras`
--
ALTER TABLE `leituras`
  ADD PRIMARY KEY (`id_leitura`),
  ADD KEY `id_sensor` (`id_sensor`);

--
-- Índices de tabela `manutencao`
--
ALTER TABLE `manutencao`
  ADD PRIMARY KEY (`id`),
  ADD KEY `Manutencao_equipamentoId_fkey` (`equipamentoId`),
  ADD KEY `Manutencao_responsavelId_fkey` (`responsavelId`);

--
-- Índices de tabela `movimentacao`
--
ALTER TABLE `movimentacao`
  ADD PRIMARY KEY (`id`),
  ADD KEY `Movimentacao_estoqueId_fkey` (`estoqueId`),
  ADD KEY `Movimentacao_usuarioId_fkey` (`usuarioId`);

--
-- Índices de tabela `ordemproducao`
--
ALTER TABLE `ordemproducao`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `numeroOp` (`numeroOp`),
  ADD KEY `OrdemProducao_produtoId_fkey` (`produtoId`);

--
-- Índices de tabela `perfil`
--
ALTER TABLE `perfil`
  ADD PRIMARY KEY (`id`);

--
-- Índices de tabela `produto`
--
ALTER TABLE `produto`
  ADD PRIMARY KEY (`id`);

--
-- Índices de tabela `sensores`
--
ALTER TABLE `sensores`
  ADD PRIMARY KEY (`id_sensor`);

--
-- Índices de tabela `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `login` (`login`),
  ADD KEY `Usuario_perfilId_fkey` (`perfilId`);

--
-- AUTO_INCREMENT para tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `alerta`
--
ALTER TABLE `alerta`
  MODIFY `id_alerta` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT de tabela `equipamento`
--
ALTER TABLE `equipamento`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de tabela `estoque`
--
ALTER TABLE `estoque`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `etapaop`
--
ALTER TABLE `etapaop`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `etapaproducao`
--
ALTER TABLE `etapaproducao`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `eventousuario`
--
ALTER TABLE `eventousuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de tabela `indicador`
--
ALTER TABLE `indicador`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de tabela `leituras`
--
ALTER TABLE `leituras`
  MODIFY `id_leitura` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=82;

--
-- AUTO_INCREMENT de tabela `manutencao`
--
ALTER TABLE `manutencao`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de tabela `movimentacao`
--
ALTER TABLE `movimentacao`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de tabela `ordemproducao`
--
ALTER TABLE `ordemproducao`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `perfil`
--
ALTER TABLE `perfil`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT de tabela `produto`
--
ALTER TABLE `produto`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `sensores`
--
ALTER TABLE `sensores`
  MODIFY `id_sensor` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de tabela `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- Restrições para tabelas despejadas
--

--
-- Restrições para tabelas `alerta`
--
ALTER TABLE `alerta`
  ADD CONSTRAINT `alerta_ibfk_1` FOREIGN KEY (`id_sensor`) REFERENCES `sensores` (`id_sensor`);

--
-- Restrições para tabelas `estoque`
--
ALTER TABLE `estoque`
  ADD CONSTRAINT `Estoque_produtoId_fkey` FOREIGN KEY (`produtoId`) REFERENCES `produto` (`id`) ON UPDATE CASCADE;

--
-- Restrições para tabelas `etapaop`
--
ALTER TABLE `etapaop`
  ADD CONSTRAINT `EtapaOP_etapaId_fkey` FOREIGN KEY (`etapaId`) REFERENCES `etapaproducao` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `EtapaOP_opId_fkey` FOREIGN KEY (`opId`) REFERENCES `ordemproducao` (`id`) ON DELETE CASCADE;

--
-- Restrições para tabelas `eventousuario`
--
ALTER TABLE `eventousuario`
  ADD CONSTRAINT `EventoUsuario_usuarioId_fkey` FOREIGN KEY (`usuarioId`) REFERENCES `usuario` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Restrições para tabelas `indicador`
--
ALTER TABLE `indicador`
  ADD CONSTRAINT `Indicador_equipamentoId_fkey` FOREIGN KEY (`equipamentoId`) REFERENCES `equipamento` (`id`) ON UPDATE CASCADE;

--
-- Restrições para tabelas `leituras`
--
ALTER TABLE `leituras`
  ADD CONSTRAINT `leituras_ibfk_1` FOREIGN KEY (`id_sensor`) REFERENCES `sensores` (`id_sensor`);

--
-- Restrições para tabelas `manutencao`
--
ALTER TABLE `manutencao`
  ADD CONSTRAINT `Manutencao_equipamentoId_fkey` FOREIGN KEY (`equipamentoId`) REFERENCES `equipamento` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `Manutencao_responsavelId_fkey` FOREIGN KEY (`responsavelId`) REFERENCES `usuario` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Restrições para tabelas `movimentacao`
--
ALTER TABLE `movimentacao`
  ADD CONSTRAINT `Movimentacao_estoqueId_fkey` FOREIGN KEY (`estoqueId`) REFERENCES `estoque` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `Movimentacao_usuarioId_fkey` FOREIGN KEY (`usuarioId`) REFERENCES `usuario` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Restrições para tabelas `ordemproducao`
--
ALTER TABLE `ordemproducao`
  ADD CONSTRAINT `OrdemProducao_produtoId_fkey` FOREIGN KEY (`produtoId`) REFERENCES `produto` (`id`) ON UPDATE CASCADE;

--
-- Restrições para tabelas `usuario`
--
ALTER TABLE `usuario`
  ADD CONSTRAINT `Usuario_perfilId_fkey` FOREIGN KEY (`perfilId`) REFERENCES `perfil` (`id`) ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
