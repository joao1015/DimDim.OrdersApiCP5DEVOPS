namespace DimDim.OrdersApi.Dtos;

public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, long TotalCount);
