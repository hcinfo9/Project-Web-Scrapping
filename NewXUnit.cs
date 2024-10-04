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

        // Testar para lista nula
        Assert.Throws<ArgumentException>(() => foodService.ProcessFoods(null));

        // Testar para lista vazia
        Assert.Throws<ArgumentException>(() => foodService.ProcessFoods(new List<FoodData>()));
    }

    [Fact]
    public void ProcessComponents_ValidData_ShouldProcessCorrectly()
    {
        // Arrange
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
                // Adicione os outros campos conforme necessário
            }
        };

        // Act
        foodService.ProcessComponents(components);

        // Assert
        mockRepository.Verify(r => r.SaveComponents(components), Times.Once);
    }


    [Fact]
    public void Constructor_ValidConnectionString_ShouldSetConnectionString()
    {
        // Arrange
        var connectionString = "Host=localhost;Port=5432;Username=user;Password=pass;Database=webfood";

        // Act
        var config = new DatabaseConfig(connectionString);

        // Assert
        Assert.Equal(connectionString, config.ConnectionString);
    }

    [Fact]
    public void Constructor_NullOrEmptyConnectionString_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DatabaseConfig(null));
        Assert.Throws<ArgumentNullException>(() => new DatabaseConfig(string.Empty));
    }

}
