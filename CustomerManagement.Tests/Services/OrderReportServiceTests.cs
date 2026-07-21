using CustomerManagement.Business.DTOs.Reports;
using CustomerManagement.Business.Services;
using CustomerManagement.Domain.Models.Reports;
using CustomerManagement.Persistence.Repositories.Interfaces;
using Moq;
using Xunit;

namespace CustomerManagement.Tests.Services;

public class OrderReportServiceTests
{
    private readonly Mock<IOrderReportRepository> _repositoryMock;
    private readonly OrderReportService _service;

    public OrderReportServiceTests()
    {
        _repositoryMock = new Mock<IOrderReportRepository>();
        _service = new OrderReportService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetCustomerOrderSummaryAsync_WhenSummaryExists_ReturnsMappedResponse()
    {
        var reportResult = new CustomerOrderSummaryResult
        {
            CustomerId = 1,
            CustomerName = "Ali Hassan",
            TotalOrders = 4,
            TotalSpent = 850m,
            LastOrderDate = new DateTime(
                2026, 7, 18, 10, 30, 0,
                DateTimeKind.Utc)
        };

        _repositoryMock
            .Setup(repository =>
                repository.GetCustomerOrderSummaryAsync(1))
            .ReturnsAsync(reportResult);

        var result =
            await _service.GetCustomerOrderSummaryAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.CustomerId);
        Assert.Equal("Ali Hassan", result.CustomerName);
        Assert.Equal(4, result.TotalOrders);
        Assert.Equal(850m, result.TotalSpent);
        Assert.Equal(reportResult.LastOrderDate, result.LastOrderDate);

        _repositoryMock.Verify(
            repository =>
                repository.GetCustomerOrderSummaryAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetCustomerOrderSummaryAsync_WhenSummaryDoesNotExist_ReturnsNull()
    {
        _repositoryMock
            .Setup(repository =>
                repository.GetCustomerOrderSummaryAsync(99))
            .ReturnsAsync((CustomerOrderSummaryResult?)null);

        var result =
            await _service.GetCustomerOrderSummaryAsync(99);

        Assert.Null(result);

        _repositoryMock.Verify(
            repository =>
                repository.GetCustomerOrderSummaryAsync(99),
            Times.Once);
    }

    [Fact]
    public async Task SearchOrdersAsync_WhenOrdersExist_ReturnsMappedResponses()
    {
        var request = new SearchOrdersRequest
        {
            CustomerId = 1,
            Status = "Completed",
            StartDate = new DateTime(
                2026, 7, 1, 0, 0, 0,
                DateTimeKind.Utc),
            EndDate = new DateTime(
                2026, 7, 31, 23, 59, 59,
                DateTimeKind.Utc)
        };

        var reportResults = new List<OrderSearchResult>
        {
            new()
            {
                OrderId = 10,
                CustomerId = 1,
                CustomerName = "Ali Hassan",
                OrderDate = new DateTime(
                    2026, 7, 10, 12, 0, 0,
                    DateTimeKind.Utc),
                Status = "Completed",
                TotalAmount = 200m
            },
            new()
            {
                OrderId = 11,
                CustomerId = 1,
                CustomerName = "Ali Hassan",
                OrderDate = new DateTime(
                    2026, 7, 15, 14, 0, 0,
                    DateTimeKind.Utc),
                Status = "Completed",
                TotalAmount = 350m
            }
        };

        _repositoryMock
            .Setup(repository =>
                repository.SearchOrdersAsync(
                    request.CustomerId,
                    request.Status,
                    request.StartDate,
                    request.EndDate))
            .ReturnsAsync(reportResults);

        var result =
            (await _service.SearchOrdersAsync(request)).ToList();

        Assert.Equal(2, result.Count);

        Assert.Equal(10, result[0].OrderId);
        Assert.Equal(1, result[0].CustomerId);
        Assert.Equal("Ali Hassan", result[0].CustomerName);
        Assert.Equal("Completed", result[0].Status);
        Assert.Equal(200m, result[0].TotalAmount);

        Assert.Equal(11, result[1].OrderId);
        Assert.Equal(350m, result[1].TotalAmount);

        _repositoryMock.Verify(
            repository =>
                repository.SearchOrdersAsync(
                    request.CustomerId,
                    request.Status,
                    request.StartDate,
                    request.EndDate),
            Times.Once);
    }

    [Fact]
    public async Task SearchOrdersAsync_WhenNoOrdersExist_ReturnsEmptyCollection()
    {
        var request = new SearchOrdersRequest();

        _repositoryMock
            .Setup(repository =>
                repository.SearchOrdersAsync(
                    null,
                    null,
                    null,
                    null))
            .ReturnsAsync(new List<OrderSearchResult>());

        var result =
            await _service.SearchOrdersAsync(request);

        Assert.Empty(result);

        _repositoryMock.Verify(
            repository =>
                repository.SearchOrdersAsync(
                    null,
                    null,
                    null,
                    null),
            Times.Once);
    }
}