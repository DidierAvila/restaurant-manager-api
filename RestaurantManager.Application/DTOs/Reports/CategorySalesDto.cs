namespace RestaurantManager.Application.DTOs.Reports
{
    public class CategorySalesDto
    {
        public string Category { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
        public decimal Percentage { get; set; }
    }
}
