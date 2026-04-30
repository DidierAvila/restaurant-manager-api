namespace RestaurantManager.Application.DTOs.Reports
{
    public class DishSalesDto
    {
        public string DishName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
    }
}
