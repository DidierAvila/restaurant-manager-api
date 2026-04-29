using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.Common.Pagination;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Infrastructure.DbContexts;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Users;

public class GetAllUsersFiltered : PaginationServiceBase<User, UserBasicDto, UserFilterDto>
{
    private readonly RestaurantManagerDbContext _context;
    private readonly IMapper _mapper;

    public GetAllUsersFiltered(RestaurantManagerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginationResponseDto<UserBasicDto>> HandleAsync(UserFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Users
            .Include(u => u.UserType)
            .Include(u => u.Roles)
            .AsQueryable();

        return await GetPaginatedAsync(query, filter, cancellationToken);
    }

    protected override IQueryable<User> ApplyFilters(IQueryable<User> query, UserFilterDto filter)
    {
        // Filtro de búsqueda general (nombre, email y teléfono)
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(u => u.Name.Contains(filter.Search) ||
                                   u.Email.Contains(filter.Search) ||
                                   (u.Phone != null && u.Phone.Contains(filter.Search)));
        }

        // Filtro por nombre específico
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(u => u.Name.Contains(filter.Name));
        }

        // Filtro por email específico
        if (!string.IsNullOrWhiteSpace(filter.Email))
        {
            query = query.Where(u => u.Email.Contains(filter.Email));
        }

        // Filtro por rol
        if (filter.RoleId.HasValue)
        {
            query = query.Where(u => u.Roles.Any(r => r.Id == filter.RoleId.Value));
        }

        // Filtro por tipo de usuario
        if (filter.UserTypeId.HasValue)
        {
            query = query.Where(u => u.UserTypeId == filter.UserTypeId.Value);
        }

        // Filtro por fecha de creación (después de)
        if (filter.CreatedAfter.HasValue)
        {
            query = query.Where(u => u.CreatedAt >= filter.CreatedAfter.Value);
        }

        // Filtro por fecha de creación (antes de)
        if (filter.CreatedBefore.HasValue)
        {
            query = query.Where(u => u.CreatedAt <= filter.CreatedBefore.Value);
        }

        return query;
    }

    protected override IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool sortDescending)
    {
        return SortingHelper.CreateSortingBuilder(query)
            .AddSortMapping("name", u => u.Name)
            .AddSortMapping("email", u => u.Email)
            .AddSortMapping("createdat", u => u.CreatedAt ?? DateTime.MinValue)
            .AddSortMapping("usertypeid", u => u.UserTypeId)
            .SetDefaultSort(u => u.Name)
            .ApplySorting(sortBy, sortDescending);
    }

    protected override async Task<IEnumerable<UserBasicDto>> MapToDto(IEnumerable<User> entities, CancellationToken cancellationToken)
    {
        IEnumerable<UserBasicDto> userTypeDtos = _mapper.Map<IEnumerable<UserBasicDto>>(entities);
        return await Task.FromResult(userTypeDtos);
    }
}
