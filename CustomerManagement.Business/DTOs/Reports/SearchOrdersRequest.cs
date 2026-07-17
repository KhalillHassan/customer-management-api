namespace CustomerManagement.Business.DTOs.Reports;

public class SearchOrdersRequest
{
    public int? CustomerId { get; set; }

    public string? Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}