using api.Model;
using MySqlConnector;

namespace api.Data
{
    public class AppDbContext
    {
        private readonly string _connectionString;

        public AppDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection String 'DefaultConnection' não encontrada.");
        }

        public MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        // ==================== USUARIO ====================

        public Usuario? BuscarUsuarioPorLogin(string login)
        {
            const string sql = @"
                SELECT u.id, u.nome, u.login, u.senhaHash,
                       u.perfilId, u.ativo, p.nome AS nomePerfil
                FROM Usuario u
                INNER JOIN Perfil p ON p.id = u.perfilId
                WHERE u.login = @login
                LIMIT 1";

            using var conn   = OpenConnection();
            using var cmd    = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Usuario
            {
                Id         = reader.GetInt32("id"),
                Nome       = reader.GetString("nome"),
                Login      = reader.GetString("login"),
                SenhaHash  = reader.GetString("senhaHash"),
                PerfilId   = reader.GetInt32("perfilId"),
                Ativo      = reader.GetBoolean("ativo"),
                NomePerfil = reader.GetString("nomePerfil")
            };
        }

        public void CriarUsuario(string nome, string login, string senhaHash, int perfilId)
        {
            const string sql = @"
                INSERT INTO Usuario (nome, login, senhaHash, perfilId, ativo)
                VALUES (@nome, @login, @senhaHash, @perfilId, TRUE)";

            using var conn = OpenConnection();
            using var cmd  = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nome",      nome);
            cmd.Parameters.AddWithValue("@login",     login);
            cmd.Parameters.AddWithValue("@senhaHash", senhaHash);
            cmd.Parameters.AddWithValue("@perfilId",  perfilId);
            cmd.ExecuteNonQuery();
        }

        // ==================== PRODUTO ====================

        public void CadastroProduto(Produto produto, Estoque estoque)
        {
            const string sqlProduto = @"
                INSERT INTO Produto (nome, descricao, preco)
                VALUES (@nome, @descricao, @preco)";

            const string sqlEstoque = @"
                INSERT INTO Estoque (produtoId, quantidade, estoqueMinimo, unidadeMedida)
                VALUES (@produtoId, @quantidade, @estoqueMinimo, @unidadeMedida)";

            using var conn        = OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                int produtoId;
                using (var cmd = new MySqlCommand(sqlProduto, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@nome",      produto.Nome);
                    cmd.Parameters.AddWithValue("@descricao", produto.Descricao);
                    cmd.Parameters.AddWithValue("@preco",     produto.Preco);
                    cmd.ExecuteNonQuery();
                    produtoId = (int)cmd.LastInsertedId;
                }

                using (var cmd = new MySqlCommand(sqlEstoque, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@produtoId",     produtoId);
                    cmd.Parameters.AddWithValue("@quantidade",    estoque.Quantidade);
                    cmd.Parameters.AddWithValue("@estoqueMinimo", estoque.EstoqueMinimo);
                    cmd.Parameters.AddWithValue("@unidadeMedida", estoque.UnidadeMedida);
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

        public List<Produto> ListarProdutos()
        {
            const string sql = @"
                SELECT p.id, p.nome, p.descricao, p.preco,
                       e.id AS estoqueId, e.produtoId, e.quantidade,
                       e.estoqueMinimo, e.unidadeMedida
                FROM Produto p
                LEFT JOIN Estoque e ON p.id = e.produtoId
                ORDER BY p.nome";

            var produtos = new List<Produto>();

            using var conn   = OpenConnection();
            using var cmd    = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var produto = new Produto(
                    reader.GetString("nome"),
                    reader.GetString("descricao"),
                    reader.GetDouble("preco")
                ) { Id = reader.GetInt32("id") };

                if (!reader.IsDBNull(reader.GetOrdinal("estoqueId")))
                {
                    produto.Estoque = new Estoque(
                        reader.GetInt32("estoqueId"),
                        reader.GetInt32("produtoId"),
                        reader.GetInt32("quantidade"),
                        reader.GetInt32("estoqueMinimo"),
                        reader.GetString("unidadeMedida")
                    );
                }

                produtos.Add(produto);
            }

            return produtos;
        }

        public void DeletarProduto(int produtoId)
        {
            const string sqlDeleteEstoque = "DELETE FROM Estoque WHERE produtoId = @id";
            const string sqlDeleteProduto = "DELETE FROM Produto WHERE id = @id";

            using var conn        = OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                using (var cmd = new MySqlCommand(sqlDeleteEstoque, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", produtoId);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new MySqlCommand(sqlDeleteProduto, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", produtoId);
                    if (cmd.ExecuteNonQuery() == 0)
                        throw new Exception("Produto não encontrado.");
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ==================== MOVIMENTACAO ====================

        public void AdicionarMovimentacao(MovimentacaoEstoque mov)
        {
            const string sqlSelect = "SELECT quantidade FROM Estoque WHERE id = @id";
            const string sqlUpdate = "UPDATE Estoque SET quantidade = @quantidade WHERE id = @id";
            const string sqlInsert = @"
                INSERT INTO Movimentacao (estoqueId, usuarioId, tipo, quantidade, dataMovimento)
                VALUES (@estoqueId, @usuarioId, @tipo, @quantidade, @dataMovimento)";

            using var conn        = OpenConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
                int quantidadeAtual;
                using (var cmd = new MySqlCommand(sqlSelect, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", mov.EstoqueId);
                    var result = cmd.ExecuteScalar()
                        ?? throw new Exception("Item de estoque não encontrado.");
                    quantidadeAtual = Convert.ToInt32(result);
                }

                if (mov.Tipo == "Saida" && quantidadeAtual < mov.Quantidade)
                    throw new Exception("Quantidade insuficiente no estoque.");

                int novaQtd = mov.Tipo == "Entrada"
                    ? quantidadeAtual + mov.Quantidade
                    : quantidadeAtual - mov.Quantidade;

                using (var cmd = new MySqlCommand(sqlUpdate, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@quantidade", novaQtd);
                    cmd.Parameters.AddWithValue("@id",         mov.EstoqueId);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new MySqlCommand(sqlInsert, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@estoqueId",     mov.EstoqueId);
                    cmd.Parameters.AddWithValue("@usuarioId",     mov.UsuarioId == 0 ? DBNull.Value : (object)mov.UsuarioId);
                    cmd.Parameters.AddWithValue("@tipo",          mov.Tipo);
                    cmd.Parameters.AddWithValue("@quantidade",    mov.Quantidade);
                    cmd.Parameters.AddWithValue("@dataMovimento", mov.DataMovimento == default ? DateTime.Now : mov.DataMovimento);
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

        public List<MovimentacaoEstoque> ListarMovimentacoes(int estoqueId)
        {
            const string sql = @"
                SELECT id, estoqueId, usuarioId, tipo, quantidade, dataMovimento
                FROM Movimentacao
                WHERE estoqueId = @estoqueId
                ORDER BY dataMovimento DESC";

            var lista = new List<MovimentacaoEstoque>();

            using var conn   = OpenConnection();
            using var cmd    = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@estoqueId", estoqueId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new MovimentacaoEstoque(
                    reader.GetInt32("id"),
                    reader.GetInt32("estoqueId"),
                    reader.IsDBNull(reader.GetOrdinal("usuarioId")) ? 0 : reader.GetInt32("usuarioId"),
                    reader.GetInt32("quantidade"),
                    reader.GetDateTime("dataMovimento"),
                    reader.GetString("tipo"),
                    string.Empty,
                    string.Empty
                ));
            }

            return lista;
        }
    }
}