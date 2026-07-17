using CustomerManagement.Business.DTOs.Reports;
using CustomerManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Api.Controllers;

[ApiController]
[Route("api/v1/order-reports")]
[Authorize]
public class OrderReportsController : ControllerBase
{
    private readonly IOrderReportService _reportService;

    public OrderReportsController(
        IOrderReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("customers/{customerId:int}/summary")]
    public async Task<IActionResult>
        GetCustomerOrderSummary(int customerId)
    {
        var summary =
            await _reportService
                .GetCustomerOrderSummaryAsync(customerId);

        if (summary is null)
        {
            return NotFound();
        }

        return Ok(summary);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchOrders(
        [FromQuery] SearchOrdersRequest request)
    {
        var orders =
            await _reportService.SearchOrdersAsync(request);

        return Ok(orders);
    }
}