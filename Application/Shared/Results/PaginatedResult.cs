namespace Application.Shared.Results;

public sealed class PaginatedResult<T>
{
    public int TotalRows { get; init; }
    public int TotalPages { get; init; }
    public int CurrentPage { get; init; }
    public int RowsPerPage { get; init; }
    public IReadOnlyList<T> Data { get; init; } = [];

    public static PaginatedResult<T> Empty() => new()
    {
        TotalRows = 0,
        TotalPages = 0,
        CurrentPage = 1,
        RowsPerPage = 0,
        Data = []
    };
}