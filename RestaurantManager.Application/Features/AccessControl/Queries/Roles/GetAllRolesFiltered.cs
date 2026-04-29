using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.Common.Pagination;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Infrastructure.DbContexts;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Roles
{
    public class GetAllRolesFiltered : PaginationServiceBase<Role, RoleListResponseDto, RoleFilterDto>
    {
        private readonly RestaurantManagerDbContext _context;
        private readonly IMapper _mapper;

    public GetAllRolesFiltered(RestaurantManagerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

        public async Task<PaginationResponseDto<RoleListResponseDto>> GetRolesFiltered(RoleFilterDto filter, CancellationToken cancellationToken)
        {
            // Usar la query directa del contexto de EF para mantener IAsyncQueryProvider
            var baseQuery = _context.Roles
                .Include(r => r.RolePermissions) // Incluir la relación real
                .ThenInclude(rp => rp.Permission) // Luego incluir Permission
                .Include(r => r.Users)       // Para UserCount
                .AsQueryable();

            return await GetPaginatedAsync(baseQuery, filter, cancellationToken);
        }

        protected override IQueryable<Role> ApplyFilters(IQueryable<Role> query, RoleFilterDto filter)
        {
            // Búsqueda general en nombre y descripción
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(searchTerm) || 
                                        (r.Description != null && r.Description.ToLower().Contains(searchTerm)));
            }
            
            // Filtro por nombre específico
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(r => r.Name.ToLower().Contains(filter.Name.ToLower()));
            }

            // Filtro por estado
            if (filter.Status.HasValue)
            {
                query = query.Where(r => r.Status == filter.Status.Value);
            }

            return query;
        }

        protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, string? sortBy, bool sortDescending)
        {
            return SortingHelper.CreateSortingBuilder(query)
                .AddSortMapping("name", r => r.Name)
                .AddSortMapping("description", r => r.Description ?? string.Empty)
                .AddSortMapping("status", r => r.Status)
                .AddSortMapping("createdat", r => r.CreatedAt)
                .SetDefaultSort(r => r.Name)
                .ApplySorting(sortBy, sortDescending);
        }

        protected override async Task<IEnumerable<RoleListResponseDto>> MapToDto(IEnumerable<Role> entities, CancellationToken cancellationToken)
        {
            var roleDtos = entities.Select(r => new RoleListResponseDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Status = r.Status,
                UserCount = r.Users?.Count ?? 0,
                PermissionCount = r.RolePermissions?.Count ?? 0, // Usar la relación real
                CreatedAt = r.CreatedAt
            }).ToList();

            return await Task.FromResult(roleDtos);
        }
    }
}
