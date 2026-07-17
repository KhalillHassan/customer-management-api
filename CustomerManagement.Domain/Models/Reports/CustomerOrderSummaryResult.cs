namespace CustomerManagement.Domain.Models.Reports;

public class CustomerOrderSummaryResult
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public long TotalOrders { get; set; }

    public decimal TotalSpent { get; set; }

    public DateTime? LastOrderDate { get; set; }
}