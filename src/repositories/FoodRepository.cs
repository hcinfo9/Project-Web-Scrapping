using Dapper;
using databasePG.Config;
using Npgsql;
using System.Data;
using foodData.Models;
using foodComponent.Models;

namespace FoodRepo.Repositories
{
    public interface IFoodRepository
    {
        void SaveFood(List<FoodData> foods);
        void SaveComponents(List<FoodComponent> components);
    }
    public class FoodRepository : IFoodRepository
    {
        private readonly string _connectionString;

        public FoodRepository(DatabaseConfig config)
        {
            _connectionString = config.ConnectionString;
        }

        // Criação de tabelas, só executada uma vez
        public void InitializeDatabase()
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();

                const string createFoodTableQuery = @"
                CREATE TABLE IF NOT EXISTS food (
                    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
                    codigo VARCHAR(50) UNIQUE,
                    nome VARCHAR(255),
                    nome_cientifico VARCHAR(255),
                    grupo VARCHAR(255)
                );";

                const string createComponentTableQuery = @"
                CREATE TABLE IF NOT EXISTS food_components (
                    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
                    codigo_alimento VARCHAR(50) REFERENCES food(codigo),
                    componente VARCHAR(255),
                    unidade VARCHAR(50),
                    valor_por_100g VARCHAR(50),
                    desvio_padrao VARCHAR(50),
                    valor_minimo VARCHAR(50),
                    valor_maximo VARCHAR(50),
                    numero_dados_utilizados VARCHAR(50),
                    referencias VARCHAR(255),
                    tipo_de_dado VARCHAR(50)
                );";

                dbConnection.Execute(createFoodTableQuery);
                dbConnection.Execute(createComponentTableQuery);
            }
        }

        // Inserir ou atualizar alimento
        public void SaveFood(List<FoodData> food)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();

                const string query = @"
                    INSERT INTO food (codigo, nome, nome_cientifico, grupo) 
                    VALUES (@Codigo, @Nome, @NomeCientifico, @Grupo)
                    ON CONFLICT (codigo) DO NOTHING;";

                dbConnection.Execute(query, food);
            }
        }

        // Inserir componentes de um alimento
        public void SaveComponents(List<FoodComponent> components)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();

                foreach (var component in components)
                {
                    // Verifica se o componente já existe na tabela para o alimento
                    const string checkQuery = @"
                        SELECT COUNT(1) 
                        FROM food_components 
                        WHERE codigo_alimento = @CodigoAlimento AND componente = @Componente AND unidade = @Unidade;";


                    int exists = dbConnection.QuerySingleOrDefault<int>(checkQuery, new
                    {
                        codigoAlimento = component.CodigoAlimento,
                        componente = component.Componente,
                        unidade = component.Unidade
                    });

                    // Se o componente já existe, pular a inserção
                    if (exists > 0)
                    {
                        Console.WriteLine($"Componente '{component.Componente}' já existe para o alimento com código {component.CodigoAlimento}. Não será inserido.");
                        continue;
                    }

                    // Se não existe, insere o componente
                    const string insertQuery = @"
                        INSERT INTO food_components 
                        (codigo_alimento, componente, unidade, valor_por_100g, desvio_padrao, valor_minimo, valor_maximo, numero_dados_utilizados, referencias, tipo_de_dado)
                        VALUES (@CodigoAlimento, @Componente, @Unidade, @ValorPor100g, @DesvioPadrao, @ValorMinimo, @ValorMaximo, @NumeroDadosUtilizados, @Referencias, @TipoDeDado);";

                    dbConnection.Execute(insertQuery, component);
                }
            }
        }

    }
}
