using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace RestaurantManager.Application.Features.Common.Pagination;

/// <summary>
/// Helper genérico para aplicar ordenamiento a consultas IQueryable
/// </summary>
public static class SortingHelper
{
    /// <summary>
    /// Aplica ordenamiento a una consulta usando el nombre de la propiedad
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="query">Consulta base</param>
    /// <param name="sortBy">Nombre de la propiedad para ordenar</param>
    /// <param name="sortDescending">Si el ordenamiento es descendente</param>
    /// <param name="defaultSort">Expresión de ordenamiento por defecto</param>
    /// <returns>Consulta con ordenamiento aplicado</returns>
    public static IQueryable<T> ApplyDynamicSorting<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool sortDescending = false,
        Expression<Func<T, object>>? defaultSort = null)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return defaultSort != null 
                ? (sortDescending ? query.OrderByDescending(defaultSort) : query.OrderBy(defaultSort))
                : query;
        }

        var propertyInfo = GetPropertyInfo<T>(sortBy);
        if (propertyInfo == null)
        {
            return defaultSort != null 
                ? (sortDescending ? query.OrderByDescending(defaultSort) : query.OrderBy(defaultSort))
                : query;
        }

        return ApplySortingByProperty(query, propertyInfo, sortDescending);
    }

    /// <summary>
    /// Aplica ordenamiento usando un diccionario de mapeo de propiedades
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="query">Consulta base</param>
    /// <param name="sortBy">Nombre de la propiedad para ordenar</param>
    /// <param name="sortDescending">Si el ordenamiento es descendente</param>
    /// <param name="sortingMap">Diccionario de mapeo de propiedades</param>
    /// <param name="defaultSort">Expresión de ordenamiento por defecto</param>
    /// <returns>Consulta con ordenamiento aplicado</returns>
    public static IQueryable<T> ApplyMappedSorting<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool sortDescending,
        Dictionary<string, Expression<Func<T, object>>> sortingMap,
        Expression<Func<T, object>>? defaultSort = null)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return defaultSort != null 
                ? (sortDescending ? query.OrderByDescending(defaultSort) : query.OrderBy(defaultSort))
                : query;
        }

        var key = sortBy.ToLowerInvariant();
        if (sortingMap.TryGetValue(key, out var sortExpression))
        {
            return sortDescending 
                ? query.OrderByDescending(sortExpression) 
                : query.OrderBy(sortExpression);
        }

        return defaultSort != null 
            ? (sortDescending ? query.OrderByDescending(defaultSort) : query.OrderBy(defaultSort))
            : query;
    }

    /// <summary>
    /// Crea un builder para configurar ordenamiento fluido
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="query">Consulta base</param>
    /// <returns>Builder de ordenamiento</returns>
    public static SortingBuilder<T> CreateSortingBuilder<T>(IQueryable<T> query)
    {
        return new SortingBuilder<T>(query);
    }

    /// <summary>
    /// Obtiene información de la propiedad por nombre (case-insensitive)
    /// </summary>
    private static PropertyInfo? GetPropertyInfo<T>(string propertyName)
    {
        return typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Aplica ordenamiento usando PropertyInfo
    /// </summary>
    private static IQueryable<T> ApplySortingByProperty<T>(IQueryable<T> query, PropertyInfo propertyInfo, bool sortDescending)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        var lambda = Expression.Lambda(propertyAccess, parameter);

        var methodName = sortDescending ? "OrderByDescending" : "OrderBy";
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), propertyInfo.PropertyType);

        return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
    }
}

/// <summary>
/// Builder para configurar ordenamiento de forma fluida
/// </summary>
/// <typeparam name="T">Tipo de entidad</typeparam>
public class SortingBuilder<T>
{
    private readonly IQueryable<T> _query;
    private readonly Dictionary<string, Expression<Func<T, object>>> _sortingMap = new();
    private Expression<Func<T, object>>? _defaultSort;

    internal SortingBuilder(IQueryable<T> query)
    {
        _query = query;
    }

    /// <summary>
    /// Añade un mapeo de ordenamiento
    /// </summary>
    /// <param name="key">Clave de ordenamiento (case-insensitive)</param>
    /// <param name="expression">Expresión de ordenamiento</param>
    /// <returns>Builder para encadenamiento</returns>
    public SortingBuilder<T> AddSortMapping(string key, Expression<Func<T, object>> expression)
    {
        _sortingMap[key.ToLowerInvariant()] = expression;
        return this;
    }

    /// <summary>
    /// Establece el ordenamiento por defecto
    /// </summary>
    /// <param name="expression">Expresión de ordenamiento por defecto</param>
    /// <returns>Builder para encadenamiento</returns>
    public SortingBuilder<T> SetDefaultSort(Expression<Func<T, object>> expression)
    {
        _defaultSort = expression;
        return this;
    }

    /// <summary>
    /// Aplica el ordenamiento configurado
    /// </summary>
    /// <param name="sortBy">Campo de ordenamiento</param>
    /// <param name="sortDescending">Si el ordenamiento es descendente</param>
    /// <returns>Consulta con ordenamiento aplicado</returns>
    public IQueryable<T> ApplySorting(string? sortBy, bool sortDescending = false)
    {
        return _query.ApplyMappedSorting(sortBy, sortDescending, _sortingMap, _defaultSort);
    }
}

/// <summary>
/// Extensiones específicas para entidades comunes
/// </summary>
public static class CommonSortingExtensions
{
    /// <summary>
    /// Aplica ordenamiento común para entidades con propiedades estándar
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="query">Consulta base</param>
    /// <param name="sortBy">Campo de ordenamiento</param>
    /// <param name="sortDescending">Si el ordenamiento es descendente</param>
    /// <returns>Consulta con ordenamiento aplicado</returns>
    public static IQueryable<T> ApplyCommonSorting<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool sortDescending = false)
    {
        // Usar ordenamiento dinámico en lugar de mapeo manual para evitar warnings
        return query.ApplyDynamicSorting(sortBy, sortDescending);
    }
}