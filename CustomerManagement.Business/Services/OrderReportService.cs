using CustomerManagement.Business.DTOs.Reports;
using CustomerManagement.Business.Interfaces;
using CustomerManagement.Persistence.Repositories.Interfaces;

namespace CustomerManagement.Business.Services;

public class OrderReportService : IOrderReportService
{
    private readonly IOrderReportRepository _repository;

    public OrderReportService(
        IOrderReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerOrderSummaryResponse?>
        GetCustomerOrderSummaryAsync(int customerId)
    {
        var result =
            await _repository
                .GetCustomerOrderSummaryAsync(customerId);

        if (result is null)
        {
            return null;
        }

        return new CustomerOrderSummaryResponse
        {
            CustomerId = result.CustomerId,
            CustomerName = result.CustomerName,
            TotalOrders = result.TotalOrders,
            TotalSpent = result.TotalSpent,
            LastOrderDate = result.LastOrderDate
        };
    }

    public async Task<IEnumerable<OrderSearchResponse>>
        SearchOrdersAsync(SearchOrdersRequest request)
    {
        var results =
            await _repository.SearchOrdersAsync(
                request.CustomerId,
                request.Status,
                request.StartDate,
                request.EndDate);

        return results.Select(result =>
            new OrderSearchResponse
            {
                OrderId = result.OrderId,
                CustomerId = result.CustomerId,
                CustomerName = result.CustomerName,
                OrderDate = result.OrderDate,
                Status = result.Status,
                TotalAmount = result.TotalAmount
            });
    }
}  