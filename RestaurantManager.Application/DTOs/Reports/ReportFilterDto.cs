namespace RestaurantManager.Application.DTOs.Reports
{
    public class ReportFilterDto
    {
        public DateTime FromDate { get; set; } = DateTime.UtcNow.AddDays(-30);
        public DateTime ToDate { get; set; } = DateTime.UtcNow;
    }
}
