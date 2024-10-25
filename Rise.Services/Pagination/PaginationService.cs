using System;
using Rise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Rise.Domain.Common;

namespace Rise.Services.Pagination;

public static class PaginationService
{
    public static async Task<ItemsPageDto<TDto>> GetPaginatedResultsAsync<TEntity, TDto>(
    this IQueryable<TEntity> query,
    Expression<Func<TEntity, bool>> filter,
    List<Expression<Func<TEntity, object>>> orderByExpressions,
    Expression<Func<TEntity, TDto>> projection,
    int? cursor,
    bool isNextPage,
    int pageSize)
    where TEntity : class
    {
        // Apply the filter
        query = query.Where(filter);

        // Apply ordering based on multiple fields
        IOrderedQueryable<TEntity>? orderedQuery = null;
        for (int i = 0; i < orderByExpressions.Count; i++)
        {
            orderedQuery = i == 0
                ? query.OrderBy(orderByExpressions[i])
                : orderedQuery.ThenBy(orderByExpressions[i]);
        }

        query = orderedQuery ?? query;

        // Determine the amount of records to take
        int takeAmount = pageSize + 1;

        // Apply cursor-based pagination
        if (cursor is not null)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var cursorProperty = Expression.Property(parameter, "Id");
            var cursorComparison = isNextPage
                ? Expression.GreaterThan(cursorProperty, Expression.Constant(cursor))
                : Expression.LessThan(cursorProperty, Expression.Constant(cursor));

            var lambda = Expression.Lambda<Func<TEntity, bool>>(cursorComparison, parameter);
            query = query.Where(lambda);

            if (!isNextPage)
            {
                takeAmount = pageSize;
            }
        }

        // Limit the result set
        query = query.Take(takeAmount);

        // Project the results to the specified DTO
        var results = await query.Select(projection).ToListAsync();

        if (results.Count == 0)
        {
            return new ItemsPageDto<TDto>
            {
                Data = new List<TDto>(),
                IsFirstPage = true,
                NextId = null,
                PreviousId = null
            };
        }

        // Determine if it’s the first page
        bool isFirstPage = !cursor.HasValue ||
                           (cursor.HasValue && results.FirstOrDefault()?.GetType().GetProperty("Id")?.GetValue(results.FirstOrDefault(), null) as int? ==
                           query.FirstOrDefault()?.GetType().GetProperty("Id")?.GetValue(query.FirstOrDefault(), null) as int?);

        // Adjust for next/previous pagination indicators
        bool hasNextPage = results.Count > pageSize || (cursor is not null && !isNextPage);

        // Remove the extra element if it exists
        if (results.Count > pageSize)
        {
            results.RemoveAt(results.Count - 1);
        }

        int? nextId = hasNextPage ? (int?)results.LastOrDefault()?.GetType().GetProperty("Id")?.GetValue(results.LastOrDefault(), null) : null;
        int? previousId = results.Count > 0 && !isFirstPage ? (int?)results.FirstOrDefault()?.GetType().GetProperty("Id")?.GetValue(results.FirstOrDefault(), null) : null;

        return new ItemsPageDto<TDto>
        {
            Data = results,
            IsFirstPage = isFirstPage,
            NextId = nextId,
            PreviousId = previousId
        };
    }

}
