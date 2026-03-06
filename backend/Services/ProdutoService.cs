using api.Model;
using MySqlConnector;

namespace EstoqueProducao.Services
{
    public class ProdutoService
    {
        private readonly string _connectionString;

        public ProdutoService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CriarProduto(string nome, string descricao, double ?preco)
        {
            if(String.IsNullOrEmpty(nome) || String.IsNullOrEmpty(descricao) || !preco.HasValue) // Função utilizada para verificar se o que foi digitado é nulo ou vazio
            {
                Console.WriteLine("O campo não pode estar vazio.");
                return;
            }

            try
            {

            using (MySqlConnection conexao = new MySqlConnection(_connectionString)) // O using garante que a conexão feche automaticamente
            {
                conexao.Open(); // Abre a conexão
                Console.WriteLine("Conexão foi aberta.");
                string sql = @"INSERT INTO Produto(nome,descricao,preco) VALUES (@nome,@descricao,@preco)"; // Parametros para serem inseridos no banco


                    using (MySqlCommand comando = new MySqlCommand(sql, conexao)) // Cria comando SQL e utiliza e utiliza o using para que o comando seja descartado
                    {
                        comando.Parameters.AddWithValue("@nome",nome);  // Substitui os placeholders por valores reais // Proteje contra SQL Injection
                        comando.Parameters.AddWithValue("@descricao", descricao);
                        comando.Parameters.AddWithValue("@preco", preco);

                        int linhasAfetadas = comando.ExecuteNonQuery(); // Retorna o numeros de linhas afetadas

                        if(linhasAfetadas > 0) // Verifica se os dados foram inseridos
                        {
                            Console.WriteLine($"Produto {nome} com a descricão: {descricao} inserido com sucesso.");
                        }
                    }
                }
            }
            catch(MySqlException ex)
            {
                 Console.WriteLine($"Erro de SQL: {ex.Message}");
                if(ex.Number == 2627)
                {
                    Console.WriteLine("Usuário já existente.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
            }
        }

        public void EntradaProduto(int ?quantidade, string categoria, int ?quantidadeMinima, string unidadeMedida)
        {
            if(quantidade.HasValue || String.IsNullOrEmpty(categoria) || !quantidadeMinima.HasValue || String.IsNullOrEmpty(unidadeMedida)) // Função utilizada para verificar se o que foi digitado é nulo ou vazio
            {
                Console.WriteLine("O campo não pode estar vazio.");
                return;
            }

            try
            {
                 using (MySqlConnection conexao = new MySqlConnection(_connectionString)) // O using garante que a conexão feche automaticamente
                {
                    conexao.Open(); // Abre a conexão
                    Console.WriteLine("Conexão foi aberta.");
                    string sql = @"INSERT INTO Estoque(quantidade,quantidadeMinima, categoria, estoqueMinimo) VALUES (@quantidade,@categoria,@estoqueMinimo, @unidadeMedida)"; // Parametros para serem inseridos no banco


                    using (MySqlCommand comando = new MySqlCommand(sql, conexao)) // Cria comando SQL e utiliza e utiliza o using para que o comando seja descartado
                    {
                        comando.Parameters.AddWithValue("@quantidade", quantidade);  // Substitui os placeholders por valores reais // Proteje contra SQL Injection
                        comando.Parameters.AddWithValue("@categoria", categoria);
                        comando.Parameters.AddWithValue("@quantidadeMinima", quantidadeMinima);
                        comando.Parameters.AddWithValue("@unidadeMedida", unidadeMedida);

                        int linhasAfetadas = comando.ExecuteNonQuery(); // Retorna o numeros de linhas afetadas

                        if(linhasAfetadas > 0) // Verifica se os dados foram inseridos
                        {
                            Console.WriteLine($"Usuario {quantidade} inserido com sucesso.");
                        }
                    }
                }
            }
            catch(MySqlException ex) // Parte de capturas de erros específicos do mysql
            {
                Console.WriteLine($"Erro de SQL: {ex.Message}");
                if(ex.Number == 2627)
                {
                    Console.WriteLine("Usuário já existente.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
            }
        }

        public void LerProdutos()
        {
            
        }
    }
}