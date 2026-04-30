using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Application.Features.Dishes.Queries;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Infrastructure.DbContexts;
using RestaurantManager.Application.Features.Common.Pagination;

namespace RestaurantManager.Application.Features.Dishes.Handlers
{
    public class DishQueryHandler :
        IRequestHandler<GetAllDishesQuery, PaginationResponseDto<DishDto>>,
        IRequestHandler<GetDishByIdQuery, DishDto?>,
        IRequestHandler<GetAvailableDishesQuery, List<DishDto>>,
        IRequestHandler<GetDishesByCategoryQuery, List<DishDto>>
    {
        private readonly IRepositoryBase<Dish> _dishRepository;
        private readonly RestaurantManagerDbContext _context;
        private readonly IMapper _mapper;

        public DishQueryHandler(IRepositoryBase<Dish> dishRepository, RestaurantManagerDbContext context, IMapper mapper)
        {
            _dishRepository = dishRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginationResponseDto<DishDto>> Handle(GetAllDishesQuery request, CancellationToken cancellationToken)
        {
            var filter = request.Filter ?? new DishFilterDto();

            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Dishes.AsNoTracking().AsQueryable();

            if (filter.Category.HasValue)
            {
                query = query.Where(d => d.Category == filter.Category.Value);
            }

            if (filter.IsAvailable.HasValue)
            {
                query = query.Where(d => d.IsAvailable == filter.IsAvailable.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                query = query.Where(d =>
                    d.Name.Contains(search) ||
                    (d.Description != null && d.Description.Contains(search)));
            }

            var totalRecords = await query.CountAsync(cancellationToken);

            var sortedQuery = SortingHelper.CreateSortingBuilder(query)
                .AddSortMapping("name", d => d.Name)
                .AddSortMapping("price", d => d.Price)
                .AddSortMapping("category", d => d.Category)
                .AddSortMapping("isavailable", d => d.IsAvailable)
                .AddSortMapping("createdat", d => d.CreatedAt)
                .AddSortMapping("updatedat", d => d.UpdatedAt)
                .SetDefaultSort(d => d.Name)
                .ApplySorting(filter.SortBy, filter.SortDescending);

            var entities = await sortedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<DishDto>>(entities);

            return dtos.ToPaginatedResult(page, pageSize, totalRecords, filter.SortBy);
        }

        public async Task<DishDto?> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByID(request.Id, cancellationToken);
            return dish != null ? _mapper.Map<DishDto>(dish) : null;
        }

        public async Task<List<DishDto>> Handle(GetAvailableDishesQuery request, CancellationToken cancellationToken)
        {
            var dishes = await _dishRepository.Finds(d => d.IsAvailable, cancellationToken);

            if (dishes == null)
                return new List<DishDto>();

            var orderedDishes = dishes
                .OrderBy(d => d.Category)
                .ThenBy(d => d.Name)
                .ToList();

            return _mapper.Map<List<DishDto>>(orderedDishes);
        }

        public async Task<List<DishDto>> Handle(GetDishesByCategoryQuery request, CancellationToken cancellationToken)
        {
            var dishes = await _dishRepository.Finds(d => d.Category == request.Category, cancellationToken);

            if (dishes == null)
                return new List<DishDto>();

            var orderedDishes = dishes
                .OrderBy(d => d.Name)
                .ToList();

            return _mapper.Map<List<DishDto>>(orderedDishes);
        }
    }
}
