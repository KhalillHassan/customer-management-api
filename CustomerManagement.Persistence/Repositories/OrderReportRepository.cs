using CustomerManagement.Domain.Models.Reports;
using CustomerManagement.Persistence.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CustomerManagement.Persistence.Repositories;

public class OrderReportRepository : IOrderReportRepository
{
    private readonly string _connectionString;

    public OrderReportRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "The DefaultConnection connection string was not found.");
    }

    public async Task<CustomerOrderSummaryResult?>
        GetCustomerOrderSummaryAsync(int customerId)
    {
        await using var connection =
            new NpgsqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        const string callProcedureSql = """
            CALL public."GetCustomerOrderSummary"
            (
                @CustomerId,
                'customer_summary_cursor'
            );
            """;

        const string fetchCursorSql = """
            FETCH ALL FROM customer_summary_cursor;
            """;

        try
        {
            await connection.ExecuteAsync(
                callProcedureSql,
                new
                {
                    CustomerId = customerId
                },
                transaction);

            var result =
                await connection
                    .QuerySingleOrDefaultAsync<
                        CustomerOrderSummaryResult>(
                            fetchCursorSql,
                            transaction: transaction);

            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<OrderSearchResult>>
        SearchOrdersAsync(
            int? customerId,
            string? status,
            DateTime? startDate,
            DateTime? endDate)
    {
        await using var connection =
            new NpgsqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        const string callProcedureSql = """
            CALL public."SearchOrders"
            (
                CAST(@CustomerId AS integer),
                CAST(@Status AS text),
                CAST(@StartDate AS timestamp with time zone),
                CAST(@EndDate AS timestamp with time zone),
                'search_orders_cursor'
            );
            """;

        const string fetchCursorSql = """
            FETCH ALL FROM search_orders_cursor;
            """;

        try
        {
            await connection.ExecuteAsync(
                callProcedureSql,
                new
                {
                    CustomerId = customerId,
                    Status = status,
                    StartDate = startDate,
                    EndDate = endDate
                },
                transaction);

            var results =
                await connection.QueryAsync<OrderSearchResult>(
                    fetchCursorSql,
                    transaction: transaction);

            await transaction.CommitAsync();

            return results;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}