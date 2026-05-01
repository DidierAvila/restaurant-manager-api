using MediatR;
using RestaurantManager.Application.DTOs.Reports;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.Reports.Queries
{
    public class GetSalesReportQuery : IRequest<Result<SalesReportDto>>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
