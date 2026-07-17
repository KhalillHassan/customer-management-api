using CustomerManagement.Domain.Models.Reports;

namespace CustomerManagement.Persistence.Repositories.Interfaces;

public interface IOrderReportRepository
{
    Task<CustomerOrderSummaryResult?>
        GetCustomerOrderSummaryAsync(int customerId);

    Task<IEnumerable<OrderSearchResult>>
        SearchOrdersAsync(
            int? customerId,
            string? status,
            DateTime? startDate,
            DateTime? endDate);
}