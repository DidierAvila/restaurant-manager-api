using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Application.DTOs.Reports;
using RestaurantManager.Application.Features.Reports.Queries;

namespace RestaurantManager.Api.Controllers
{
    /// <summary>
    /// Controlador para reportes de ventas del restaurante
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReportsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Generar reporte de ventas por rango de fechas
        /// </summary>
        /// <param name="fromDate">Fecha inicial (formato: yyyy-MM-dd)</param>
        /// <param name="toDate">Fecha final (formato: yyyy-MM-dd)</param>
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = new GetSalesReportQuery
            {
                FromDate = fromDate ?? DateTime.UtcNow.AddDays(-30),
                ToDate = toDate ?? DateTime.UtcNow
            };

            var result = await _mediator.Send(query);
            return HandleResult(result);
        }
    }
}
