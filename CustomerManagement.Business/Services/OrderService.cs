using CustomerManagement.Business.DTOs.Orders;
using CustomerManagement.Business.Interfaces;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Persistence.Interfaces;
using CustomerManagement.Persistence.Repositories.Interfaces;

namespace CustomerManagement.Business.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders =
            await _unitOfWork.Orders.GetAllWithDetailsAsync();

        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        var order =
            await _unitOfWork.Orders.GetByIdWithDetailsAsync(id);

        if (order is null)
        {
            return null;
        }

        return MapToResponse(order);
    }
    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.FullName,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            IsActive = order.IsActive,

            Items = order.OrderItems
                .Select(item => new OrderItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Subtotal = item.Quantity * item.UnitPrice
                })
                .ToList()
        };
    }

    public async Task<OrderResponse> CreateAsync(
    CreateOrderRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException(
                "An order must contain at least one item.");
        }

        var customer =
            await _unitOfWork.Customers.GetByIdAsync(
                request.CustomerId);

        if (customer is null || customer.IsDeleted)
        {
            throw new KeyNotFoundException(
                "Customer was not found.");
        }

        await using var transaction =
            await _unitOfWork.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                CustomerId = customer.Id,
                Customer = customer,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = 0,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            foreach (var requestedItem in request.Items)
            {
                if (requestedItem.Quantity <= 0)
                {
                    throw new ArgumentException(
                        "Product quantity must be greater than zero.");
                }

                var product =
                    await _unitOfWork.Products.GetByIdAsync(
                        requestedItem.ProductId);

                if (product is null ||
                    product.IsDeleted ||
                    !product.IsActive)
                {
                    throw new KeyNotFoundException(
                        $"Product {requestedItem.ProductId} was not found.");
                }

                if (product.StockQuantity < requestedItem.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock for product {product.Name}.");
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = requestedItem.Quantity,
                    UnitPrice = product.Price,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                };

                order.OrderItems.Add(orderItem);

                order.TotalAmount +=
                    product.Price * requestedItem.Quantity;

                product.StockQuantity -=
                    requestedItem.Quantity;

                product.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.Products.Update(product);
            }

            await _unitOfWork.Orders.AddAsync(order);

            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync();

            var savedOrder =
                await _unitOfWork.Orders
                    .GetByIdWithDetailsAsync(order.Id);

            if (savedOrder is null)
            {
                throw new InvalidOperationException(
                    "The order was saved but could not be loaded.");
            }

            return MapToResponse(savedOrder);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order =
            await _unitOfWork.Orders.GetByIdAsync(id);

        if (order is null || order.IsDeleted)
        {
            return false;
        }

        order.IsDeleted = true;
        order.IsActive = false;
        order.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Orders.Update(order);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateStatusAsync(
    int id,
    UpdateOrderStatusRequest request)
    {
        var allowedStatuses = new[]
        {
        "Pending",
        "Processing",
        "Completed",
        "Cancelled"
    };

        var normalizedStatus = allowedStatuses
            .FirstOrDefault(status =>
                status.Equals(
                    request.Status,
                    StringComparison.OrdinalIgnoreCase));

        if (normalizedStatus is null)
        {
            throw new ArgumentException(
                "Status must be Pending, Processing, Completed, or Cancelled.");
        }

        var order =
            await _unitOfWork.Orders.GetByIdAsync(id);

        if (order is null || order.IsDeleted)
        {
            return false;
        }

        order.Status = normalizedStatus;
        order.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Orders.Update(order);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}