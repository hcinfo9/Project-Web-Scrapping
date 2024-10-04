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

        /// <summary>
        /// Realiza o scraping dos dados de alimentos e componentes.
        /// </summary>
        /// <param name="url">URL da página a ser raspada.</param>
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

                        // Criação do objeto FoodData
                        var foodData = new FoodData
                        {
                            Codigo = codigo,
                            Nome = nome,
                            NomeCientifico = nomeCientificoText,
                            Grupo = grupoText
                        };

                        foodDataList.Add(foodData);

                        // Salvar alimentos no banco via FoodService
                        _foodService.ProcessFoods(foodDataList);

                        // Scraping dos componentes desse alimento
                        var components = ScrapeComponents(codigo);

                        // Salvar componentes no banco via FoodService
                        _foodService.ProcessComponents(components);
                    }
                }

                // Salvar alimentos no banco via FoodService
                _foodService.ProcessFoods(foodDataList);
            }
            else
            {
                Console.WriteLine("Nenhum dado encontrado na tabela de alimentos.");
            }
        }

        /// <summary>
        /// Realiza o scraping dos componentes de um alimento específico.
        /// </summary>
        /// <param name="codigoAlimento">Código do alimento.</param>
        /// <returns>Lista de componentes do alimento.</returns>
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
                    var unidadeNode = node.SelectSingleNode("./td[2]");
                    var valorNode = node.SelectSingleNode(".//td[3]");
                    var desvioNode = node.SelectSingleNode(".//td[4]");
                    var valorMinimoNode = node.SelectSingleNode(".//td[5]");
                    var valorMaximoNode = node.SelectSingleNode(".//td[6]");
                    var numeroDadosUtilizadosNode = node.SelectSingleNode(".//td[7]");
                    var referenciasNode = node.SelectSingleNode(".//td[8]");
                    var tipoDadoNode = node.SelectSingleNode(".//td[9]");

                    if (componenteNode != null && unidadeNode != null && valorNode != null)
                    {
                        var componente = componenteNode.InnerText.Trim();
                        var unidade = unidadeNode.InnerHtml.Trim();
                        var valor = valorNode.InnerText.Trim();
                        var desvio = desvioNode.InnerHtml.Trim();
                        var valorMinimo = valorMinimoNode.InnerHtml.Trim();
                        var valorMaximo = valorMaximoNode.InnerHtml.Trim();
                        var numeroDadosUtilizados = numeroDadosUtilizadosNode.InnerHtml.Trim();
                        var referencias = referenciasNode.InnerHtml.Trim();
                        var tipoDado = tipoDadoNode.InnerHtml.Trim();

                        var foodComponent = new FoodComponent
                        {
                            CodigoAlimento = codigoAlimento,
                            Componente = componente,
                            Unidade = unidade,
                            ValorPor100g = valor,
                            DesvioPadrao = desvio,
                            ValorMinimo = valorMinimo,
                            ValorMaximo = valorMaximo,
                            NumeroDadosUtilizados = numeroDadosUtilizados,
                            Referencias = referencias,
                            TipoDeDado = tipoDado
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
