namespace foodComponent.Models
{
    /// <summary>
    /// Representa um componente nutricional de um alimento.
    /// </summary>
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
