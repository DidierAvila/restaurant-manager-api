using MediatR;
using RestaurantManager.Application.DTOs.Reports;

namespace RestaurantManager.Application.Features.Reports.Queries
{
    public class GetSalesReportQuery : IRequest<SalesReportDto>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
