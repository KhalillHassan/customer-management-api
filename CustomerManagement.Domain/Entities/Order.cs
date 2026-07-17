using CustomerManagement.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagement.Domain.Entities;

public class Order : BaseEntity
{
    public int CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public string Status { get; set; }
        = string.Empty;

    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem> OrderItems
    {
        get;
        set;
    } = new List<OrderItem>();
}