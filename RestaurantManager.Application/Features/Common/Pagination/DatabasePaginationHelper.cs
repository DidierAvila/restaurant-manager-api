using Microsoft.EntityFrameworkCore;
using RestaurantManager.Application.DTOs.Common;
using System.Linq.Expressions;

namespace RestaurantManager.Application.Features.Common.Pagination;

/// <summary>
/// Helper para optimizar consultas de paginación a nivel de base de datos
/// </summary>
public static class DatabasePaginationHelper
{
    /// <summary>
    /// Aplica paginación optimizada directamente en la base de datos
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="query">Query base</param>
    /// <param name="page">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="sortBy">Campo de ordenamiento</param>
    /// <param name="sortDescending">Si el ordenamiento es descendente</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado paginado</returns>
    public static async Task<(IEnumerable<T> Data, int TotalCount)> GetPaginatedDataAsync<T>(
        IQueryable<T> query,
        int page,
        int pageSize,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
        where T : class
    {
        // Normalizar parámetros
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        // Contar total (optimizado para evitar cargar datos)
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar ordenamiento
        var sortedQuery = query.ApplyCommonSorting(sortBy, sortDescending);

        // Aplicar paginación
        var skip = (page - 1) * pageSize;
        var data = await sortedQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (data, totalCount);
    }

    /// <summary>
    /// Crea una respuesta paginada optimizada
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad</typeparam>
    /// <typeparam name="TDto">Tipo de DTO</typeparam>
    /// <param name="query">Query base</param>
    /// <param name="filter">Filtros de paginación</param>
    /// <param name="mapToDto">Función de mapeo a DTO</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta paginada</returns>
    public static async Task<PaginationResponseDto<TDto>> CreatePaginatedResponseAsync<TEntity, TDto>(
        IQueryable<TEntity> query,
        PaginationRequestDto filter,
        Func<IEnumerable<TEntity>, Task<IEnumerable<TDto>>> mapToDto,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var (data, totalCount) = await GetPaginatedDataAsync(
            query,
            filter.Page,
            filter.PageSize,
            filter.SortBy,
            filter.SortDescending,
            cancellationToken);

        var dtoData = await mapToDto(data);

        return dtoData.ToPaginatedResult(
            filter.Page,
            filter.PageSize,
            totalCount,
            filter.SortBy);
    }

    /// <summary>
    /// Aplica filtros de búsqueda optimizados usando índices de texto completo si están disponibles
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="query">Query base</param>
    /// <param name="searchTerm">Término de búsqueda</param>
    /// <param name="searchProperties">Propiedades donde buscar</param>
    /// <returns>Query con filtro de búsqueda aplicado</returns>
    public static IQueryable<T> ApplyTextSearch<T>(
        this IQueryable<T> query,
        string? searchTerm,
        params Expression<Func<T, string>>[] searchProperties)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchProperties.Length == 0)
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var propertySelector in searchProperties)
        {
            var property = Expression.Invoke(propertySelector, parameter);
            
            // Usar EF.Functions.Like para mejor rendimiento en SQL Server
            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
                nameof(DbFunctionsExtensions.Like),
                [typeof(DbFunctions), typeof(string), typeof(string)]);

            if (likeMethod != null)
            {
                var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));
                var pattern = Expression.Constant($"%{searchTerm}%");
                var likeExpression = Expression.Call(likeMethod, efFunctions, property, pattern);

                combinedExpression = combinedExpression == null
                    ? likeExpression
                    : Expression.OrElse(combinedExpression, likeExpression);
            }
            else
            {
                // Fallback a Contains si Like no está disponible
                var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
                var searchConstant = Expression.Constant(searchTerm);
                var containsExpression = Expression.Call(property, containsMethod!, searchConstant);

                combinedExpression = combinedExpression == null
                    ? containsExpression
                    : Expression.OrElse(combinedExpression, containsExpression);
            }
        }

        if (combinedExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    /// <summary>
    /// Aplica filtros de fecha optimizados
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="query">Query base</param>
    /// <param name="dateFrom">Fecha desde</param>
    /// <param name="dateTo">Fecha hasta</param>
    /// <param name="dateProperty">Propiedad de fecha</param>
    /// <returns>Query con filtro de fecha aplicado</returns>
    public static IQueryable<T> ApplyDateRangeFilter<T>(
        this IQueryable<T> query,
        DateTime? dateFrom,
        DateTime? dateTo,
        Expression<Func<T, DateTime>> dateProperty)
    {
        if (dateFrom.HasValue)
        {
            query = query.Where(CombinePropertyComparison(dateProperty, dateFrom.Value, ExpressionType.GreaterThanOrEqual));
        }

        if (dateTo.HasValue)
        {
            // Ajustar fecha hasta para incluir todo el día
            var dateToEndOfDay = dateTo.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(CombinePropertyComparison(dateProperty, dateToEndOfDay, ExpressionType.LessThanOrEqual));
        }

        return query;
    }

    /// <summary>
    /// Combina una propiedad con un valor usando el tipo de comparación especificado
    /// </summary>
    private static Expression<Func<T, bool>> CombinePropertyComparison<T, TProperty>(
        Expression<Func<T, TProperty>> propertySelector,
        TProperty value,
        ExpressionType comparisonType)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Invoke(propertySelector, parameter);
        var constant = Expression.Constant(value);
        var comparison = Expression.MakeBinary(comparisonType, property, constant);

        return Expression.Lambda<Func<T, bool>>(comparison, parameter);
    }

    /// <summary>
    /// Valida y normaliza parámetros de paginación
    /// </summary>
    /// <param name="filter">Filtro de paginación</param>
    public static void ValidatePaginationParams(PaginationRequestDto filter)
    {
        if (filter.Page <= 0) filter.Page = 1;
        if (filter.PageSize <= 0) filter.PageSize = 10;
        if (filter.PageSize > 100) filter.PageSize = 100;
    }
}