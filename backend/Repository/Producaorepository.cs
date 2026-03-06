using api.Model;
using MySqlConnector;

namespace api.Data
{
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

        public void CadastrarEtapaProducao(EtapaProducao etapa)
        {
            string sql = @"INSERT INTO EtapaProducao (nomeEtapa, descricao, ordem)
                           VALUES (@nomeEtapa, @descricao, @ordem)";

            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@nomeEtapa", etapa.NomeEtapa);
            cmd.Parameters.AddWithValue("@descricao", etapa.Descricao);
            cmd.Parameters.AddWithValue("@ordem", etapa.Ordem);

            cmd.ExecuteNonQuery();
        }

        public List<EtapaProducao> ListarEtapasProducao()
        {
            string sql = "SELECT id, nomeEtapa, descricao, ordem FROM EtapaProducao ORDER BY ordem";

            var etapas = new List<EtapaProducao>();

            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                etapas.Add(new EtapaProducao(
                    reader.GetInt32("id"),
                    reader.GetString("nomeEtapa"),
                    reader.GetString("descricao"),
                    reader.GetInt32("ordem")
                ));
            }

            return etapas;
        }

        public void DeletarEtapaProducao(int id)
        {
            string sql = "DELETE FROM EtapaProducao WHERE id = @id";

            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            int linhas = cmd.ExecuteNonQuery();
            if (linhas == 0)
                throw new Exception("Etapa não encontrada.");
        }


        public void CriarOrdemProducao(OrdemProducao op)
        {
            string sqlOP = @"INSERT INTO OrdemProducao (numeroOp, produtoId, quantidade, status)
                             VALUES (@numeroOP, @produtoId, @quantidade, @status)";

            string sqlLastId = "SELECT LAST_INSERT_ID()";

            string sqlBuscarEtapas = "SELECT id FROM EtapaProducao ORDER BY ordem";

            string sqlEtapaOP = @"INSERT INTO EtapaOP (opId, etapaId, status)
                                  VALUES (@opId, @etapaId, @status)";

            using var conn = OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                using (var cmd = new MySqlCommand(sqlOP, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@numeroOP", op.NumeroOP);
                    cmd.Parameters.AddWithValue("@produtoId", op.ProdutoId);
                    cmd.Parameters.AddWithValue("@quantidade", op.Quantidade);
                    cmd.Parameters.AddWithValue("@status", op.Status);
                    cmd.ExecuteNonQuery();
                }

                int opId;
                using (var cmd = new MySqlCommand(sqlLastId, conn, transaction))
                {
                    opId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                var etapaIds = new List<int>();
                using (var cmd = new MySqlCommand(sqlBuscarEtapas, conn, transaction))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        etapaIds.Add(reader.GetInt32("id"));
                }

                if (etapaIds.Count == 0)
                    throw new Exception("Nenhuma EtapaProducao cadastrada. Cadastre as etapas antes de criar uma OP.");

                foreach (var etapaId in etapaIds)
                {
                    using var cmd = new MySqlCommand(sqlEtapaOP, conn, transaction);
                    cmd.Parameters.AddWithValue("@opId", opId);
                    cmd.Parameters.AddWithValue("@etapaId", etapaId);
                    cmd.Parameters.AddWithValue("@status", "Pendente");
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

        public List<OrdemProducao> ListarOrdensProducao()
        {
            string sql = @"SELECT op.id, op.numeroOp, op.produtoId, op.quantidade, op.status,
                                  p.nome AS produtoNome
                           FROM OrdemProducao op
                           INNER JOIN Produto p ON op.produtoId = p.id
                           ORDER BY op.numeroOp DESC";

            var ordens = new List<OrdemProducao>();

            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var op = new OrdemProducao(
                    reader.GetInt32("id"),
                    reader.GetInt32("numeroOp"),
                    reader.GetInt32("produtoId"),
                    reader.GetInt32("quantidade"),
                    reader.GetString("status")
                );

                op.Produto = new Produto(reader.GetString("produtoNome"), string.Empty, 0);

                ordens.Add(op);
            }

            return ordens;
        }


        public void IniciarEtapa(int etapaOpId)
        {
            string sql = @"UPDATE EtapaOP 
                           SET status = 'EmAndamento', dataInicio = @dataInicio
                           WHERE id = @id AND status = 'Pendente'";

            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@dataInicio", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@id", etapaOpId);

            int linhas = cmd.ExecuteNonQuery();
            if (linhas == 0)
                throw new Exception("Etapa não encontrada ou já foi iniciada.");
        }

        public void ConcluirEtapa(int etapaOpId)
        {
            string sql = @"UPDATE EtapaOP 
                           SET status = 'Concluido', dataFim = @dataFim
                           WHERE id = @id AND status = 'EmAndamento'";

            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@dataFim", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@id", etapaOpId);

            int linhas = cmd.ExecuteNonQuery();
            if (linhas == 0)
                throw new Exception("Etapa não encontrada ou não está em andamento.");
        }

        public List<EtapaOP> ListarEtapasDaOP(int opId)
        {
            string sql = @"SELECT eo.id, eo.opId, eo.etapaId, eo.status, eo.dataInicio, eo.dataFim,
                                  ep.nomeEtapa, ep.descricao, ep.ordem
                           FROM EtapaOP eo
                           INNER JOIN EtapaProducao ep ON eo.etapaId = ep.id
                           WHERE eo.opId = @opId
                           ORDER BY ep.ordem";

            var etapas = new List<EtapaOP>();

            using var conn = OpenConnection();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@opId", opId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var etapaOP = new EtapaOP(
                    reader.GetInt32("id"),
                    reader.GetInt32("opId"),
                    reader.GetInt32("etapaId")
                );

                etapaOP.Status = reader.GetString("status");
                etapaOP.Data_Inicio = reader.IsDBNull(reader.GetOrdinal("dataInicio"))
                    ? null
                    : reader.GetDateTime("dataInicio");
                etapaOP.Data_Fim = reader.IsDBNull(reader.GetOrdinal("dataFim"))
                    ? null
                    : reader.GetDateTime("dataFim");

                etapaOP.EtapaProducao = new EtapaProducao(
                    reader.GetInt32("etapaId"),
                    reader.GetString("nomeEtapa"),
                    reader.GetString("descricao"),
                    reader.GetInt32("ordem")
                );

                etapas.Add(etapaOP);
            }

            return etapas;
        }
    }
}