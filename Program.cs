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

        // URL para o scraping
        string url = "https://www.tbca.net.br/base-dados/composicao_estatistica.php";

        // Executar o scraping e salvar os dados no banco de dados
        scrapingService.ScrapeData(url);
    }
}
