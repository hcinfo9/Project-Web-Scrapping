
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
