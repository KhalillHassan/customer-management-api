namespace CustomerManagement.Business.DTOs.Reports;

public class CustomerOrderSummaryResponse
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public long TotalOrders { get; set; }

    public decimal TotalSpent { get; set; }

    public DateTime? LastOrderDate { get; set; }
}