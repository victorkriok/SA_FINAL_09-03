using api.Model;
using MySqlConnector;

namespace api.Repository
{
    /// <summary>
    /// Repositório responsável por todas as operações de banco de dados
    /// relacionadas a Produção: EtapaProducao, OrdemProducao e EtapaOP.
    /// </summary>
    public class ProducaoRepository
    {
        private readonly string _connectionString;

        public ProducaoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection String 'DefaultConnection' não encontrada.");
        }

        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        // ==================== ETAPAS DE PRODUCAO ====================

        public List<EtapaProducao> ListarEtapasProducao()
        {
            const string sql = @"
                SELECT id, nomeEtapa, descricao, ordem
                FROM EtapaProducao
                ORDER BY ordem";

            var lista = new List<EtapaProducao>();

            using var conn   = OpenConnection();
            using var cmd    = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                lista.Add(MapEtapaProducao(reader));

            return lista;
        }

        public void CadastrarEtapaProducao(EtapaProducao etapa)
        {
            const string sql = @"
                INSERT INTO EtapaProducao (nomeEtapa, descricao, ordem)
                VALUES (@nomeEtapa, @descricao, @ordem)";

            using var conn = OpenConnection();
            using var cmd  = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@nomeEtapa", etapa.NomeEtapa);
            cmd.Parameters.AddWithValue("@descricao", etapa.Descricao ?? string.Empty);
            cmd.Parameters.AddWithValue("@ordem",     etapa.Ordem);
            cmd.ExecuteNonQuery();
        }

        public void DeletarEtapaProducao(int id)
        {
            const string sql = "DELETE FROM EtapaProducao WHERE id = @id";

            using var conn = OpenConnection();
            using var cmd  = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            if (cmd.ExecuteNonQuery() == 0)
                throw new Exception($"Etapa não encontrada com id {id}.");
        }

        // ==================== ORDENS DE PRODUCAO ====================

        public List<OrdemProducao> ListarOrdensProducao()
        {
            const string sql = @"
                SELECT id, numeroOp, produtoId, quantidade, status
                FROM OrdemProducao
                ORDER BY id DESC";

            var lista = new List<OrdemProducao>();

            using var conn   = OpenConnection();
            using var cmd    = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                lista.Add(MapOrdemProducao(reader));

            return lista;
        }

        /// <summary>
        /// Cria uma OrdemProducao e gera automaticamente uma EtapaOP
        /// para cada EtapaProducao cadastrada no sistema.
        /// Lança exceção se não houver nenhuma EtapaProducao cadastrada.
        /// </summary>
        public void CriarOrdemProducao(OrdemProducao op)
        {
            // Busca todas as etapas existentes para gerar as EtapaOPs
            var etapas = ListarEtapasProducao();

            if (etapas.Count == 0)
                throw new Exception("Nenhuma EtapaProducao cadastrada. Cadastre etapas antes de criar uma OP.");

            const string sqlOP = @"
                INSERT INTO OrdemProducao (numeroOp, produtoId, quantidade, status)
                VALUES (@numeroOp, @produtoId, @quantidade, 'Aberta')";

            const string sqlEtapaOP = @"
                INSERT INTO EtapaOP (opId, etapaId, status)
                VALUES (@opId, @etapaId, 'Pendente')";

            using var conn        = OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                int opId;

                using (var cmd = new MySqlCommand(sqlOP, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@numeroOp",  op.NumeroOP);
                    cmd.Parameters.AddWithValue("@produtoId", op.ProdutoId);
                    cmd.Parameters.AddWithValue("@quantidade", op.Quantidade);
                    cmd.ExecuteNonQuery();
                    opId = (int)cmd.LastInsertedId;
                }

                // Cria uma EtapaOP para cada etapa cadastrada
                foreach (var etapa in etapas)
                {
                    using var cmd = new MySqlCommand(sqlEtapaOP, conn, transaction);
                    cmd.Parameters.AddWithValue("@opId",    opId);
                    cmd.Parameters.AddWithValue("@etapaId", etapa.Id);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ==================== ETAPAS DA OP ====================

        public List<EtapaOP> ListarEtapasDaOP(int opId)
        {
            const string sql = @"
                SELECT eo.id, eo.opId, eo.etapaId, eo.status,
                       eo.dataInicio, eo.dataFim,
                       ep.nomeEtapa, ep.ordem
                FROM EtapaOP eo
                INNER JOIN EtapaProducao ep ON ep.id = eo.etapaId
                WHERE eo.opId = @opId
                ORDER BY ep.ordem";

            var lista = new List<EtapaOP>();

            using var conn   = OpenConnection();
            using var cmd    = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@opId", opId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var etapaOP = new EtapaOP(
                    reader.GetInt32("id"),
                    reader.GetInt32("opId"),
                    reader.GetInt32("etapaId")
                )
                {
                    Status      = reader.GetString("status"),
                    Data_Inicio = reader.IsDBNull(reader.GetOrdinal("dataInicio"))
                                    ? null : reader.GetDateTime("dataInicio"),
                    Data_Fim    = reader.IsDBNull(reader.GetOrdinal("dataFim"))
                                    ? null : reader.GetDateTime("dataFim"),
                    // Popula a navegação com o nome da etapa para o frontend exibir
                    EtapaProducao = new EtapaProducao(
                        reader.GetInt32("etapaId"),
                        reader.GetString("nomeEtapa"),
                        string.Empty,
                        reader.GetInt32("ordem")
                    )
                };

                lista.Add(etapaOP);
            }

            return lista;
        }

        /// <summary>
        /// Inicia uma EtapaOP: muda status para "EmAndamento" e registra dataInicio.
        /// Regras: etapa deve existir e estar "Pendente".
        /// </summary>
        public void IniciarEtapa(int etapaOpId)
        {
            var etapa = BuscarEtapaOP(etapaOpId)
                ?? throw new Exception("EtapaOP não encontrada.");

            if (etapa.Status != "Pendente")
                throw new Exception($"A etapa já foi iniciada (status atual: {etapa.Status}).");

            const string sql = @"
                UPDATE EtapaOP
                SET status = 'EmAndamento', dataInicio = @agora
                WHERE id = @id";

            using var conn = OpenConnection();
            using var cmd  = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@agora", DateTime.Now);
            cmd.Parameters.AddWithValue("@id",    etapaOpId);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Conclui uma EtapaOP: muda status para "Concluida" e registra dataFim.
        /// Regras: etapa deve estar "EmAndamento".
        /// Após concluir todas as etapas, atualiza status da OP para "Concluida".
        /// </summary>
        public void ConcluirEtapa(int etapaOpId)
        {
            var etapa = BuscarEtapaOP(etapaOpId)
                ?? throw new Exception("EtapaOP não encontrada.");

            if (etapa.Status != "EmAndamento")
                throw new Exception($"A etapa não está em andamento (status atual: {etapa.Status}).");

            using var conn        = OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                // Conclui a etapa
                const string sqlConcluir = @"
                    UPDATE EtapaOP
                    SET status = 'Concluida', dataFim = @agora
                    WHERE id = @id";

                using (var cmd = new MySqlCommand(sqlConcluir, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@agora", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id",    etapaOpId);
                    cmd.ExecuteNonQuery();
                }

                // Verifica se todas as etapas da OP foram concluídas
                const string sqlCheck = @"
                    SELECT COUNT(*) FROM EtapaOP
                    WHERE opId = @opId AND status != 'Concluida'";

                int pendentes;
                using (var cmd = new MySqlCommand(sqlCheck, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@opId", etapa.OpId);
                    pendentes = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Se não há mais etapas pendentes, fecha a OP
                if (pendentes == 0)
                {
                    const string sqlFecharOP = @"
                        UPDATE OrdemProducao SET status = 'Concluida'
                        WHERE id = @opId";

                    using var cmd = new MySqlCommand(sqlFecharOP, conn, transaction);
                    cmd.Parameters.AddWithValue("@opId", etapa.OpId);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ==================== HELPERS PRIVADOS ====================

        private EtapaOP? BuscarEtapaOP(int id)
        {
            const string sql = "SELECT id, opId, etapaId, status, dataInicio, dataFim FROM EtapaOP WHERE id = @id";

            using var conn   = OpenConnection();
            using var cmd    = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new EtapaOP(reader.GetInt32("id"), reader.GetInt32("opId"), reader.GetInt32("etapaId"))
            {
                Status      = reader.GetString("status"),
                Data_Inicio = reader.IsDBNull(reader.GetOrdinal("dataInicio")) ? null : reader.GetDateTime("dataInicio"),
                Data_Fim    = reader.IsDBNull(reader.GetOrdinal("dataFim"))    ? null : reader.GetDateTime("dataFim")
            };
        }

        private static EtapaProducao MapEtapaProducao(MySqlDataReader r) =>
            new(r.GetInt32("id"), r.GetString("nomeEtapa"), r.GetString("descricao"), r.GetInt32("ordem"));

        private static OrdemProducao MapOrdemProducao(MySqlDataReader r) =>
            new(r.GetInt32("id"), r.GetInt32("numeroOp"), r.GetInt32("produtoId"),
                r.GetInt32("quantidade"), r.GetString("status"));
    }
}