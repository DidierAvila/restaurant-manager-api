namespace RestaurantManager.Application.DTOs.Reports
{
    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageTicket { get; set; }
        public TopDishDto? TopDish { get; set; }
        public List<CategorySalesDto> SalesByCategory { get; set; } = new();
        public List<DishSalesDto> SalesByDish { get; set; } = new();
    }
}
