namespace foodData.Models
{
    /// <summary>
    /// Representa os dados principais de um alimento.
    /// </summary>
    public class FoodData
    {
        public string? Codigo { get; set; }
        public string? Nome { get; set; }
        public string? NomeCientifico { get; set; }
        public string? Grupo { get; set; }
    }
}
