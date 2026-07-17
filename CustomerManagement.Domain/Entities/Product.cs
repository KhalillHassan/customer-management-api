using CustomerManagement.Domain.Common;

namespace CustomerManagement.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
        = string.Empty;

    public string Description { get; set; }
        = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public ICollection<OrderItem> OrderItems
    {
        get;
        set;
    } = new List<OrderItem>();
}