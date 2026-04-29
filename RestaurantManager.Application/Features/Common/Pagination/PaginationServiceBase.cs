using Microsoft.EntityFrameworkCore;
using RestaurantManager.Application.DTOs.Common;
using System.Linq.Expressions;

namespace RestaurantManager.Application.Features.Common.Pagination;

/// <summary>
/// Servicio base genérico para operaciones de paginación y filtrado
/// </summary>
/// <typeparam name="TEntity">Tipo de entidad</typeparam>
/// <typeparam name="TDto">Tipo de DTO de respuesta</typeparam>
/// <typeparam name="TFilter">Tipo de filtro</typeparam>
public abstract class PaginationServiceBase<TEntity, TDto, TFilter>
    where TEntity : class
    where TFilter : PaginationRequestDto
{
    /// <summary>
    /// Obtiene una respuesta paginada aplicando filtros, ordenamiento y paginación
    /// </summary>
    /// <param name="queryable">Query base de la entidad</param>
    /// <param name="filter">Filtros y parámetros de paginación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta paginada</returns>
    public async Task<PaginationResponseDto<TDto>> GetPaginatedAsync(
        IQueryable<TEntity> queryable,
        TFilter filter,
        CancellationToken cancellationToken = default)
    {
        // Validar y normalizar parámetros de paginación
        ValidateAndNormalizePaginationParams(filter);

        // Aplicar filtros específicos
        var filteredQuery = ApplyFilters(queryable, filter);

        // Contar total de registros
        var totalRecords = await filteredQuery.CountAsync(cancellationToken);

        // Aplicar ordenamiento
        var sortedQuery = ApplySorting(filteredQuery, filter.SortBy, GetSortDescending(filter));

        // Aplicar paginación
        var paginatedEntities = await sortedQuery
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        // Mapear a DTOs
        var dtos = await MapToDto(paginatedEntities, cancellationToken);

        // Crear respuesta paginada
        return CreatePaginationResponse(dtos, filter, totalRecords);
    }

    /// <summary>
    /// Obtiene una respuesta paginada usando Expression para filtros más eficientes
    /// </summary>
    /// <param name="queryable">Query base de la entidad</param>
    /// <param name="filter">Filtros y parámetros de paginación</param>
    /// <param name="additionalFilter">Expresión de filtro adicional</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta paginada</returns>
    public async Task<PaginationResponseDto<TDto>> GetPaginatedWithExpressionAsync(
        IQueryable<TEntity> queryable,
        TFilter filter,
        Expression<Func<TEntity, bool>>? additionalFilter = null,
        CancellationToken cancellationToken = default)
    {
        // Validar y normalizar parámetros de paginación
        ValidateAndNormalizePaginationParams(filter);

        // Aplicar filtro adicional si se proporciona
        if (additionalFilter != null)
        {
            queryable = queryable.Where(additionalFilter);
        }

        // Aplicar filtros específicos
        var filteredQuery = ApplyFilters(queryable, filter);

        // Contar total de registros
        var totalRecords = await filteredQuery.CountAsync(cancellationToken);

        // Aplicar ordenamiento
        var sortedQuery = ApplySorting(filteredQuery, filter.SortBy, GetSortDescending(filter));

        // Aplicar paginación
        var paginatedEntities = await sortedQuery
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        // Mapear a DTOs
        var dtos = await MapToDto(paginatedEntities, cancellationToken);

        // Crear respuesta paginada
        return CreatePaginationResponse(dtos, filter, totalRecords);
    }

    /// <summary>
    /// Valida y normaliza los parámetros de paginación
    /// </summary>
    /// <param name="filter">Filtros de paginación</param>
    protected virtual void ValidateAndNormalizePaginationParams(TFilter filter)
    {
        if (filter.Page <= 0) filter.Page = 1;
        if (filter.PageSize <= 0) filter.PageSize = 10;
        if (filter.PageSize > 100) filter.PageSize = 100;
    }

    /// <summary>
    /// Crea la respuesta de paginación
    /// </summary>
    /// <param name="data">Datos paginados</param>
    /// <param name="filter">Filtros aplicados</param>
    /// <param name="totalRecords">Total de registros</param>
    /// <returns>Respuesta paginada</returns>
    protected virtual PaginationResponseDto<TDto> CreatePaginationResponse(
        IEnumerable<TDto> data,
        TFilter filter,
        int totalRecords)
    {
        return data.ToPaginatedResult(
            filter.Page,
            filter.PageSize,
            totalRecords,
            filter.SortBy);
    }

    /// <summary>
    /// Aplica filtros específicos a la consulta. Debe ser implementado por las clases derivadas.
    /// </summary>
    /// <param name="query">Consulta base</param>
    /// <param name="filter">Filtros a aplicar</param>
    /// <returns>Consulta con filtros aplicados</returns>
    protected abstract IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query, TFilter filter);

    /// <summary>
    /// Aplica ordenamiento a la consulta. Puede ser sobrescrito para ordenamientos personalizados.
    /// </summary>
    /// <param name="query">Consulta base</param>
    /// <param name="sortBy">Campo de ordenamiento</param>
    /// <param name="sortDescending">Indica si el ordenamiento es descendente</param>
    /// <returns>Consulta con ordenamiento aplicado</returns>
    protected abstract IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, string? sortBy, bool sortDescending);

    /// <summary>
    /// Mapea las entidades a DTOs. Debe ser implementado por las clases derivadas.
    /// </summary>
    /// <param name="entities">Entidades a mapear</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>DTOs mapeados</returns>
    protected abstract Task<IEnumerable<TDto>> MapToDto(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Obtiene el valor de ordenamiento descendente del filtro. Puede ser sobrescrito.
    /// </summary>
    /// <param name="filter">Filtro</param>
    /// <returns>True si el ordenamiento es descendente</returns>
    protected virtual bool GetSortDescending(TFilter filter)
    {
        // Por defecto, buscar propiedad SortDescending usando reflexión
        var property = typeof(TFilter).GetProperty("SortDescending");
        if (property != null && property.PropertyType == typeof(bool))
        {
            return (bool)(property.GetValue(filter) ?? false);
        }
        return false;
    }
}