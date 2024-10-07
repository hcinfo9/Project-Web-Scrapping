using Xunit; // Biblioteca para testes unitários
using food.Services;
using FoodRepo.Repositories;
using Moq; // Biblioteca para criação de objetos mock
using foodData.Models;
using foodComponent.Models;
using databasePG.Config;

public class FoodServiceTests
{
    // Teste para verificar se o método ProcessFoods processa corretamente dados válidos
    [Fact]
    public void ProcessFoods_ValidData_ShouldProcessCorrectly()
    {

        var mockRepository = new Mock<IFoodRepository>();

        var foodService = new FoodService(mockRepository.Object);
        // Cria uma lista de dados de alimentos para teste
        var foods = new List<FoodData>{
            new FoodData {
                Codigo = "123",
                Nome = "Arroz",
                NomeCientifico = "Oryza sativa",
                Grupo = "Grãos"
            }
        };

        // Chama o método ProcessFoods com a lista de alimentos
        foodService.ProcessFoods(foods);

        // Verifica se o método SaveFood foi chamado uma vez com qualquer lista de FoodData
        mockRepository.Verify(r => r.SaveFood(It.IsAny<List<FoodData>>()), Times.Once);
    }

    // Teste para verificar se o método ProcessFoods lança ArgumentException para listas nulas ou vazias
    [Fact]
    public void ProcessFoods_NullOrEmptyList_ShouldThrowArgumentException()
    {

        var mockRepository = new Mock<IFoodRepository>();

        var foodService = new FoodService(mockRepository.Object);

        // Verifica se o método ProcessFoods lança ArgumentException quando passado null
        Assert.Throws<ArgumentException>(() => foodService.ProcessFoods(null));

        // Verifica se o método ProcessFoods lança ArgumentException quando passado uma lista vazia
        Assert.Throws<ArgumentException>(() => foodService.ProcessFoods(new List<FoodData>()));
    }

    // Teste para verificar se o método ProcessComponents processa corretamente dados válidos
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

        // Chama o método ProcessComponents com a lista de componentes
        foodService.ProcessComponents(components);

        // Verifica se o método SaveComponents foi chamado uma vez com a lista de componentes
        mockRepository.Verify(r => r.SaveComponents(components), Times.Once);
    }

    // Teste para verificar se o construtor da classe DatabaseConfig define corretamente a string de conexão
    [Fact]
    public void Constructor_ValidConnectionString_ShouldSetConnectionString()
    {

        var connectionString = "Host=localhost;Port=5432;Username=user;Password=pass;Database=webfood";


        var config = new DatabaseConfig(connectionString);

        // Verifica se a string de conexão foi definida corretamente
        Assert.Equal(connectionString, config.ConnectionString);
    }

    // Teste para verificar se o construtor da classe DatabaseConfig lança ArgumentNullException para strings de conexão nulas ou vazias
    [Fact]
    public void Constructor_NullOrEmptyConnectionString_ShouldThrowArgumentNullException()
    {

        Assert.Throws<ArgumentNullException>(() => new DatabaseConfig(null));

        Assert.Throws<ArgumentNullException>(() => new DatabaseConfig(string.Empty));
    }
}
