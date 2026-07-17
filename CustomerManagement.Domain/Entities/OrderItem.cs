using CustomerManagement.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagement.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; }
        = null!;

    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
        = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}