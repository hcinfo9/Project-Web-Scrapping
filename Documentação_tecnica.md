# Documentação Técnica da Aplicação

# Visão Geral
A aplicação é um sistema de scraping de dados alimentares que coleta informações de uma fonte online e armazena esses dados em um banco de dados PostgreSQL. A aplicação é composta por vários componentes, incluindo serviços de scraping, repositórios de dados, configuração de banco de dados e testes unitários.

# Estrutura do Projeto

   1) Configuração do Banco de Dados
      - DatabaseConfig.cs: Define a configuração do banco de dados, incluindo a string de conexão.


   2) Modelos de Dados
      - FoodData.cs: Representa os dados principais de um alimento.
      - FoodComponent.cs: Representa um componente nutricional de um alimento.


   3) Repositórios
      - FoodRepository.cs: Implementa a interface IFoodRepository para operações de banco de dados relacionadas a alimentos e seus componentes.
        
   
   4) Serviços
      - FoodService.cs: Fornece métodos para processar e salvar dados de alimentos e componentes.
      - ScrapingService.cs: Realiza o scraping de dados de alimentos e componentes de uma fonte online.


   5) Testes Unitários
      - FoodServiceTests.cs: Contém testes unitários para verificar o comportamento dos serviços de alimentos.
     
  
   6) Configuração do Docker
      - docker-compose.yml: Define a configuração do contêiner Docker para o banco de dados PostgreSQL.


   7) Programa Principal
      - Program.cs: Ponto de entrada da aplicação que inicializa os serviços e executa o scraping.
     


# Detalhamento dos Arquivos
 
 
 ## DatabaseConfig.cs
```bash
  namespace databasePG.Config
  {
      public class DatabaseConfig
      {
          public string ConnectionString { get; }
  
          public DatabaseConfig(string? connectionString)
          {
              if (string.IsNullOrEmpty(connectionString))
              {
                  throw new ArgumentNullException("Connection string cannot be null or empty.");
              }
  
              ConnectionString = connectionString;
          }
      }
  }
```
- Descrição: Classe de configuração do banco de dados que valida e armazena a string de conexão


## FoodData.cs

```bash
  namespace foodData.Models
  {
      public class FoodData
      {
          public string? Codigo { get; set; }
          public string? Nome { get; set; }
          public string? NomeCientifico { get; set; }
          public string? Grupo { get; set; }
      }
  }
```
- Descrição: Modelo que representa os dados principais de um alimento.


## FoodComponent.cs
  
```bash
  namespace foodComponent.Models
  {
    public class FoodComponent
    {
        public string? CodigoAlimento { get; set; }
        public string? Componente { get; set; }
        public string? Unidade { get; set; }
        public string? ValorPor100g { get; set; }
        public string? DesvioPadrao { get; set; }
        public string? ValorMinimo { get; set; }
        public string? ValorMaximo { get; set; }
        public string? NumeroDadosUtilizados { get; set; }
        public string? Referencias { get; set; }
        public string? TipoDeDado { get; set; }
    }
  }

```
- Descrição: Modelo que representa um componente nutricional de um alimento.


 # FoodRepository.cs

 ```bask
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
  
          public void SaveComponents(List<FoodComponent> components)
          {
              using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
              {
                  dbConnection.Open();
  
                  foreach (var component in components)
                  {
                      const string checkQuery = @"
                      SELECT COUNT(1) 
                      FROM food_components 
                      WHERE codigo_alimento = @CodigoAlimento AND componente = @Componente";
  
                      int exists = dbConnection.QuerySingle<int>(checkQuery, new { component.CodigoAlimento, component.Componente });
  
                      if (exists > 0)
                      {
                          Console.WriteLine($"Componente {component.Componente} já existe para o alimento {component.CodigoAlimento}.");
                          continue;
                      }
  
                      const string query = @"
                      INSERT INTO food_components (codigo_alimento, componente, unidade, valor_por_100g, desvio_padrao, valor_minimo, valor_maximo, numero_dados_utilizados, referencias, tipo_de_dado)
                      VALUES (@CodigoAlimento, @Componente, @Unidade, @ValorPor100g, @DesvioPadrao, @ValorMinimo, @ValorMaximo, @NumeroDadosUtilizados, @Referencias, @TipoDeDado);";
  
                      dbConnection.Execute(query, component);
                  }
              }
          }
      }
  }
 ```
- Descrição: Repositório que implementa operações de banco de dados para alimentos e componentes, incluindo a inicialização do banco de dados e métodos para salvar alimentos e componentes.


# FoodService.cs

```bash
  using foodComponent.Models;
  using foodData.Models;
  using FoodRepo.Repositories;
  
  namespace food.Services
  {
      public class FoodService
      {
          private readonly IFoodRepository _foodRepository;
  
          public FoodService(IFoodRepository repository)
          {
              _foodRepository = repository;
          }
  
          public void ProcessFoods(List<FoodData>? foods)
          {
              if (foods == null || foods.Count == 0)
              {
                  throw new ArgumentException("A lista de alimentos não pode estar vazia");
              }
  
              _foodRepository.SaveFood(foods);
              Console.WriteLine($"Alimento '{foods.Count}' salvo no banco de dados.");
          }
  
          public void ProcessComponents(List<FoodComponent> components)
          {
              _foodRepository.SaveComponents(components);
              Console.WriteLine("Componentes salvos no banco de dados.");
          }
      }
  }

```
  - Descrição: Serviço que processa e salva dados de alimentos e componentes, utilizando o repositório de alimentos.


## ScrappingService.cs

```bash
  using HtmlAgilityPack;
  using foodData.Models;
  using foodComponent.Models;
  using food.Services;
  
  namespace Scraping.Services
  {
      public class ScrapingService
      {
          private readonly FoodService _foodService;
  
          public ScrapingService(FoodService foodService)
          {
              _foodService = foodService;
          }
  
          public void ScrapeData(string url)
          {
              HtmlWeb web = new HtmlWeb();
              HtmlDocument doc = web.Load(url);
  
              var nodes = doc.DocumentNode.SelectNodes("//table//tr");
  
              if (nodes != null)
              {
                  var foodDataList = new List<FoodData>();
  
                  foreach (var node in nodes)
                  {
                      var codigoNode = node.SelectSingleNode(".//td[1]");
                      var nomeAlimento = node.SelectSingleNode(".//td[2]");
                      var nomeCientifico = node.SelectSingleNode(".//td[3]");
                      var grupo = node.SelectSingleNode(".//td[4]");
  
                      if (codigoNode != null && nomeAlimento != null && nomeCientifico != null && grupo != null)
                      {
                          var codigo = codigoNode.InnerText.Trim();
                          var nome = nomeAlimento.InnerText.Trim();
                          var nomeCientificoText = nomeCientifico.InnerText.Trim();
                          var grupoText = grupo.InnerText.Trim();
  
                          var foodData = new FoodData
                          {
                              Codigo = codigo,
                              Nome = nome,
                              NomeCientifico = nomeCientificoText,
                              Grupo = grupoText
                          };
  
                          foodDataList.Add(foodData);
                      }
                  }
  
                  _foodService.ProcessFoods(foodDataList);
              }
              else
              {
                  Console.WriteLine("Nenhum dado encontrado na tabela de alimentos.");
              }
          }
  
          public List<FoodComponent> ScrapeComponents(string codigoAlimento)
          {
              var urlDetails = $"https://www.tbca.net.br/base-dados/int_composicao_estatistica.php?cod_produto={codigoAlimento}";
  
              HtmlWeb web = new HtmlWeb();
              HtmlDocument doc = web.Load(urlDetails);
  
              var nodes = doc.DocumentNode.SelectNodes("//table[@id='tabela1']//tr");
              var components = new List<FoodComponent>();
  
              if (nodes != null)
              {
                  foreach (var node in nodes)
                  {
                      var componenteNode = node.SelectSingleNode(".//td[1]");
                      var unidadeNode = node.SelectSingleNode(".//td[2]");
                      var valorNode = node.SelectSingleNode(".//td[3]");
  
                      if (componenteNode != null && unidadeNode != null && valorNode != null)
                      {
                          var componente = componenteNode.InnerText.Trim();
                          var unidade = unidadeNode.InnerText.Trim();
                          var valorPor100g = valorNode.InnerText.Trim();
  
                          var foodComponent = new FoodComponent
                          {
                              CodigoAlimento = codigoAlimento,
                              Componente = componente,
                              Unidade = unidade,
                              ValorPor100g = valorPor100g
                          };
  
                          components.Add(foodComponent);
                      }
                  }
              }
              else
              {
                  Console.WriteLine($"Nenhuma tabela de componentes encontrada para o alimento {codigoAlimento}.");
              }
  
              return components;
          }
      }
  }
```
- Descrição: Serviço que realiza o scraping de dados de alimentos e componentes de uma fonte online e utiliza o FoodService para processar e salvar esses dados.


## FoodServiceTests.cs

  ```bash
    using Xunit;
    using food.Services;
    using FoodRepo.Repositories;
    using Moq;
    using foodData.Models;
    using foodComponent.Models;
    using databasePG.Config;
    
    public class FoodServiceTests
    {
        [Fact]
        public void ProcessFoods_ValidData_ShouldProcessCorrectly()
        {
            var mockRepository = new Mock<IFoodRepository>();
            var foodService = new FoodService(mockRepository.Object);
            var foods = new List<FoodData>{
                new FoodData {
                    Codigo = "123",
                    Nome = "Arroz",
                    NomeCientifico = "Oryza sativa",
                    Grupo = "Grãos"
                }
            };
    
            foodService.ProcessFoods(foods);
    
            mockRepository.Verify(r => r.SaveFood(It.IsAny<List<FoodData>>()), Times.Once);
        }
    
        [Fact]
        public void ProcessFoods_NullOrEmptyList_ShouldThrowArgumentException()
        {
            var mockRepository = new Mock<IFoodRepository>();
            var foodService = new FoodService(mockRepository.Object);
    
            Assert.Throws<ArgumentException>(() => foodService.ProcessFoods(null));
            Assert.Throws<ArgumentException>(() => foodService.ProcessFoods(new List<FoodData>()));
        }
    
        [Fact]
        public void ProcessComponents_ValidData_ShouldProcessCorrectly()
        {
            var mockRepository = new Mock<IFoodRepository>();
            var foodService = new FoodService(mockRepository.Object);
            var components = new List<FoodComponent>
            {
                new FoodComponent
                {
                    CodigoAlimento = "123",
                    Componente = "Carboidrato",
                    Unidade = "g",
                    ValorPor100g = "12.5",
                    DesvioPadrao = "0.2"
                }
            };
    
            foodService.ProcessComponents(components);
    
            mockRepository.Verify(r => r.SaveComponents(components), Times.Once);
        }
    
        [Fact]
        public void Constructor_ValidConnectionString_ShouldSetConnectionString()
        {
            var connectionString = "Host=localhost;Port=5432;Username=user;Password=pass;Database=webfood";
            var config = new DatabaseConfig(connectionString);
            Assert.Equal(connectionString, config.ConnectionString);
        }
    
        [Fact]
        public void Constructor_NullOrEmptyConnectionString_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DatabaseConfig(null));
            Assert.Throws<ArgumentNullException>(() => new DatabaseConfig(string.Empty));
        }
    }
  ```
- Descrição: Testes unitários para verificar o comportamento dos serviços de alimentos, incluindo a validação de dados e a interação com o repositório.


## Docker-compose.yml

```bash
  version: '3.8'

  services:
    pg:
      image: bitnami/postgresql:13.16.0
      container_name: WebScrapping-food
      ports:
        - 5432:5432
      environment:
        - POSTGRES_USER=docWeb
        - POSTGRES_PASSWORD=docWeb@123
        - POSTGRES_DB=webfood
      volumes:
        - pgdata:/bitnami/postgresql
      restart: unless-stopped
  
  volumes:
    pgdata:
```
- Descrição: Arquivo de configuração do Docker Compose para iniciar um contêiner PostgreSQL com as configurações necessárias.

# Program.cs

```bash
  using databasePG.Config;
  using FoodRepo.Repositories;
  using food.Services;
  using Scraping.Services;
  
  class Program
  {
      static void Main(string[] args)
      {
          var databaseConfig = new DatabaseConfig("Host=localhost;Port=5432;Username=docWeb;Password=docWeb@123;Database=webfood");
          var foodRepository = new FoodRepository(databaseConfig);
          foodRepository.InitializeDatabase();
  
          var foodService = new FoodService(foodRepository);
          var scrapingService = new ScrapingService(foodService);
  
          string url = "https://www.tbca.net.br/base-dados/composicao_estatistica.php";
          scrapingService.ScrapeData(url);
      }
  }
```
- Descrição: Ponto de entrada da aplicação que inicializa os serviços e executa o scraping de dados alimentares.


# Fluxo de Funcionamento
  
  1) Inicialização:
     - O programa principal (Program.cs) inicializa a configuração do banco de dados e os repositórios.
     - O banco de dados é inicializado com as tabelas necessárias.


  2) Scraping de Dados:
     - O ScrapingService realiza o scraping dos dados de alimentos e componentes de uma URL fornecida.
     - Os dados extraidos são processados pelo FoodService e salvos no banco de dados através do FoodRepository.
    
    
  3) Processamento de Dados:
     - O FoodService valida e processa os dados de alimentos e componentes, garantindo que os dados sejam salvos corretamente no banco de dados.


  4) Testes Unitários:
      - Os testes unitários (FoodServiceTests.cs) garantem que os serviços de alimentos funcionem corretamente, validando a lógica de negócios e a interação com o repositório.
  
