namespace CustomerManagement.Business.DTOs.Reports;

public class OrderSearchResponse
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
}