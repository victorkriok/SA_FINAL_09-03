using api.Model;
using MySqlConnector;

namespace api.Services
{
    public class ProdutoService
    {
        private readonly string _connectionString;

        public ProdutoService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CriarProduto(string nome, string descricao, double? preco)
        {
            if (String.IsNullOrEmpty(nome) || String.IsNullOrEmpty(descricao) || !preco.HasValue)
            {
                Console.WriteLine("O campo não pode estar vazio.");
                return;
            }

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_connectionString))
                {
                    conexao.Open();
                    Console.WriteLine("Conexão foi aberta.");
                    string sql = @"INSERT INTO Produto (nome, descricao, preco) VALUES (@nome, @descricao, @preco)";

                    using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                    {
                        comando.Parameters.AddWithValue("@nome", nome);
                        comando.Parameters.AddWithValue("@descricao", descricao);
                        comando.Parameters.AddWithValue("@preco", preco);

                        int linhasAfetadas = comando.ExecuteNonQuery();
                        if (linhasAfetadas > 0)
                            Console.WriteLine($"Produto {nome} com a descrição: {descricao} inserido com sucesso.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro de SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
            }
        }

        public void EntradaProduto(int? quantidade, int? quantidadeMinima, string unidadeMedida)
        {
            if (!quantidade.HasValue || !quantidadeMinima.HasValue || String.IsNullOrEmpty(unidadeMedida))
            {
                Console.WriteLine("O campo não pode estar vazio.");
                return;
            }

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_connectionString))
                {
                    conexao.Open();
                    Console.WriteLine("Conexão foi aberta.");
                    string sql = @"INSERT INTO Estoque (quantidade, estoqueMinimo, unidadeMedida) 
                                   VALUES (@quantidade, @estoqueMinimo, @unidadeMedida)";

                    using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                    {
                        comando.Parameters.AddWithValue("@quantidade", quantidade);
                        comando.Parameters.AddWithValue("@estoqueMinimo", quantidadeMinima);
                        comando.Parameters.AddWithValue("@unidadeMedida", unidadeMedida);

                        int linhasAfetadas = comando.ExecuteNonQuery();
                        if (linhasAfetadas > 0)
                            Console.WriteLine("Estoque inserido com sucesso.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro de SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
            }
        }

        public List<Produto> LerProdutos()
        {
            var produtos = new List<Produto>();

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(_connectionString))
                {
                    conexao.Open();
                    string sql = @"SELECT p.id, p.nome, p.descricao, p.preco,
                                          e.id AS estoqueId, e.produtoId, e.quantidade, e.estoqueMinimo, e.unidadeMedida
                                   FROM Produto p
                                   LEFT JOIN Estoque e ON p.id = e.produtoId";

                    using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                    using (MySqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var produto = new Produto(
                                reader.GetString("nome"),
                                reader.GetString("descricao"),
                                reader.GetDouble("preco")
                            );

                            produto.Id = reader.GetInt32("id");

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
                            Console.WriteLine($"Produto: {produto.Nome} | Qtd: {produto.Estoque?.Quantidade ?? 0} {produto.Estoque?.UnidadeMedida}");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro de SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
            }

            return produtos;
        }
    }
}