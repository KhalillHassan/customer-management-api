using CustomerManagement.Domain.Common;

namespace CustomerManagement.Domain.Entities;

public class Customer : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}