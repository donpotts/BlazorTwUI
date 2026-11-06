namespace BlazorTwUI.Models;

/// <summary>What a server-driven <c>TwDataGrid</c> asks its data provider for. <see cref="Search"/> is passed through verbatim.</summary>
public record GridDataRequest(int Skip, int Take, string? Search, string? SortBy, bool Descending);

/// <summary>One page of rows plus the total number of rows matching the request (before paging).</summary>
public record GridDataResult<TItem>(IReadOnlyList<TItem> Items, int TotalCount);
