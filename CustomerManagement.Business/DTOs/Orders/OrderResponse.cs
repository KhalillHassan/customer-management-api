namespace CustomerManagement.Business.DTOs.Orders;

public class OrderResponse
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public bool IsActive { get; set; }

    public List<OrderItemResponse> Items { get; set; }
        = new();
}