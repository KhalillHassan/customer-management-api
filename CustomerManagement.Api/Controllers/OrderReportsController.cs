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

    /// <summary>
    /// Gets an order summary for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>
    /// The customer's total orders, total spending,
    /// and most recent order date.
    /// </returns>
    [HttpGet("customers/{customerId:int}/summary")]
    [ProducesResponseType(
        typeof(CustomerOrderSummaryResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Searches orders using optional filters.
    /// </summary>
    /// <param name="request">
    /// Optional customer, status, start-date,
    /// and end-date filters.
    /// </param>
    /// <returns>A collection of matching orders.</returns>
    [HttpGet("search")]
    [ProducesResponseType(
        typeof(IEnumerable<OrderSearchResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchOrders(
        [FromQuery] SearchOrdersRequest request)
    {
        var orders =
            await _reportService.SearchOrdersAsync(request);

        return Ok(orders);
    }
}