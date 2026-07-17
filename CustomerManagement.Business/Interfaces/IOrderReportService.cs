using CustomerManagement.Business.DTOs.Reports;

namespace CustomerManagement.Business.Interfaces;

public interface IOrderReportService
{
    Task<CustomerOrderSummaryResponse?>
        GetCustomerOrderSummaryAsync(int customerId);

    Task<IEnumerable<OrderSearchResponse>>
        SearchOrdersAsync(SearchOrdersRequest request);
}