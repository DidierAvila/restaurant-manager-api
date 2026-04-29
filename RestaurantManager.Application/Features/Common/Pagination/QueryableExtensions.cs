using System.Linq.Expressions;

namespace RestaurantManager.Application.Features.Common.Pagination;

/// <summary>
/// Extensiones para IQueryable que facilitan filtrado común
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Aplica filtro de rango de fechas
    /// </summary>
    public static IQueryable<T> ApplyDateRangeFilter<T>(
        this IQueryable<T> query,
        DateTime? fromDate,
        DateTime? toDate,
        Expression<Func<T, DateTime>> dateSelector)
    {
        if (fromDate.HasValue)
        {
            var fromConstant = Expression.Constant(fromDate.Value);
            var fromBody = Expression.GreaterThanOrEqual(dateSelector.Body, fromConstant);
            var fromLambda = Expression.Lambda<Func<T, bool>>(fromBody, dateSelector.Parameters[0]);
            query = query.Where(fromLambda);
        }

        if (toDate.HasValue)
        {
            var toConstant = Expression.Constant(toDate.Value);
            var toBody = Expression.LessThanOrEqual(dateSelector.Body, toConstant);
            var toLambda = Expression.Lambda<Func<T, bool>>(toBody, dateSelector.Parameters[0]);
            query = query.Where(toLambda);
        }

        return query;
    }

    /// <summary>
    /// Aplica filtro de texto en múltiples propiedades
    /// </summary>
    public static IQueryable<T> ApplyTextSearch<T>(
        this IQueryable<T> query,
        string? searchTerm,
        params Expression<Func<T, string>>[] propertySelectors)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || propertySelectors.Length == 0)
            return query;

        var parameter = Expression.Parameter(typeof(T));
        var searchConstant = Expression.Constant(searchTerm.ToLower());

        Expression? combinedExpression = null;

        foreach (var propertySelector in propertySelectors)
        {
            var property = Expression.Invoke(propertySelector, parameter);
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var propertyToLower = Expression.Call(property, toLowerMethod!);
            var containsExpression = Expression.Call(propertyToLower, containsMethod!, searchConstant);

            combinedExpression = combinedExpression == null 
                ? containsExpression 
                : Expression.OrElse(combinedExpression, containsExpression);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression!, parameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Aplica filtro condicional
    /// </summary>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Aplica filtro si el string no está vacío
    /// </summary>
    public static IQueryable<T> WhereIfNotEmpty<T>(
        this IQueryable<T> query,
        string? value,
        Expression<Func<T, bool>> predicate)
    {
        return !string.IsNullOrWhiteSpace(value) ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Aplica filtro si el valor nullable tiene valor
    /// </summary>
    public static IQueryable<T> WhereIfHasValue<T, TValue>(
        this IQueryable<T> query,
        TValue? value,
        Expression<Func<T, bool>> predicate)
        where TValue : struct
    {
        return value.HasValue ? query.Where(predicate) : query;
    }
}