CREATE OR REPLACE PROCEDURE public."GetCustomerOrderSummary"
(
    IN p_customer_id integer,
    INOUT p_cursor refcursor
)
LANGUAGE plpgsql
AS $$ 
BEGIN

    OPEN p_cursor FOR

    SELECT
        customer."Id" AS "CustomerId",
        customer."FullName" AS "CustomerName",
        COUNT(order_record."Id") AS "TotalOrders",
        COALESCE(SUM(order_record."TotalAmount"), 0) AS "TotalSpent",
        MAX(order_record."OrderDate") AS "LastOrderDate"

    FROM public."Customers" AS customer

    LEFT JOIN public."Orders" AS order_record
        ON order_record."CustomerId" = customer."Id"
        AND order_record."IsDeleted" = false

    WHERE customer."Id" = p_customer_id
        AND customer."IsDeleted" = false

    GROUP BY
        customer."Id",
        customer."FullName";

END;
$$;


CREATE OR REPLACE PROCEDURE public."SearchOrders"
(
    IN p_customer_id integer,
    IN p_status text,
    IN p_start_date timestamp with time zone,
    IN p_end_date timestamp with time zone,
    INOUT p_cursor refcursor
)
LANGUAGE plpgsql
AS $$
BEGIN

    OPEN p_cursor FOR

    SELECT
        order_record."Id" AS "OrderId",
        order_record."CustomerId",
        customer."FullName" AS "CustomerName",
        order_record."OrderDate",
        order_record."Status",
        order_record."TotalAmount"

    FROM public."Orders" AS order_record

    INNER JOIN public."Customers" AS customer
        ON customer."Id" = order_record."CustomerId"

    WHERE order_record."IsDeleted" = false
        AND customer."IsDeleted" = false

        AND
        (
            p_customer_id IS NULL
            OR order_record."CustomerId" = p_customer_id
        )

        AND
        (
            p_status IS NULL
            OR LOWER(order_record."Status") = LOWER(p_status)
        )

        AND
        (
            p_start_date IS NULL
            OR order_record."OrderDate" >= p_start_date
        )

        AND
        (
            p_end_date IS NULL
            OR order_record."OrderDate" <= p_end_date
        )

    ORDER BY order_record."OrderDate" DESC;

END;
$$;