using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RestaurantManager.Application.DTOs.Common;

public class PaginationRequestDto
{
    /// <summary>
    /// Número de página a obtener (mínimo: 1)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    [DefaultValue(1)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Cantidad de registros por página (1-100)
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Campo por el cual ordenar los resultados (ascendente). Campos disponibles varían por entidad.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Indica si el ordenamiento es descendente
    /// </summary>
    [DefaultValue(false)]
    public bool SortDescending { get; set; } = false;
}

/// <summary>
/// Filtro base con búsqueda general
/// </summary>
public class SearchablePaginationRequestDto : PaginationRequestDto
{
    /// <summary>
    /// Término de búsqueda general
    /// </summary>
    public string? Search { get; set; }
}

/// <summary>
/// Filtro base con rango de fechas
/// </summary>
public class DateRangePaginationRequestDto : PaginationRequestDto
{
    /// <summary>
    /// Fecha de creación desde
    /// </summary>
    public DateTime? CreatedFrom { get; set; }

    /// <summary>
    /// Fecha de creación hasta
    /// </summary>
    public DateTime? CreatedTo { get; set; }
}

/// <summary>
/// Filtro base completo con búsqueda y rango de fechas
/// </summary>
public class FullPaginationRequestDto : SearchablePaginationRequestDto
{
    /// <summary>
    /// Fecha de creación desde
    /// </summary>
    public DateTime? CreatedFrom { get; set; }

    /// <summary>
    /// Fecha de creación hasta
    /// </summary>
    public DateTime? CreatedTo { get; set; }

    /// <summary>
    /// Filtrar por estado activo/inactivo
    /// </summary>
    public bool? Status { get; set; }
}

public class PaginationResponseDto<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public string? SortBy { get; set; }
}

public static class PaginationExtensions
{
    public static PaginationResponseDto<T> ToPaginatedResult<T>(
        this IEnumerable<T> source,
        int page,
        int pageSize,
        int totalRecords,
        string? sortBy = null)
    {
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        return new PaginationResponseDto<T>
        {
            Data = source,
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize,
            SortBy = sortBy
        };
    }
}
