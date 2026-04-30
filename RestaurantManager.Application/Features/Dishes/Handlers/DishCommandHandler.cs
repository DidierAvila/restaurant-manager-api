using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Application.Features.Dishes.Commands;
using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Domain.Errors;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.Dishes.Handlers
{
    public class DishCommandHandler :
        IRequestHandler<CreateDishCommand, Result<DishDto>>,
        IRequestHandler<UpdateDishCommand, Result<DishDto>>,
        IRequestHandler<DeleteDishCommand, Result>,
        IRequestHandler<ToggleDishAvailabilityCommand, Result<DishDto>>
    {
        private readonly IRepositoryBase<Dish> _dishRepository;
        private readonly IRepositoryBase<OrderItem> _orderItemRepository;
        private readonly IMapper _mapper;

        public DishCommandHandler(
            IRepositoryBase<Dish> dishRepository,
            IRepositoryBase<OrderItem> orderItemRepository,
            IMapper mapper)
        {
            _dishRepository = dishRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<Result<DishDto>> Handle(CreateDishCommand request, CancellationToken cancellationToken)
        {
            // Validar que el nombre no exista
            var existingDish = await _dishRepository.Find(
                d => d.Name.ToLower() == request.Name.ToLower(),
                cancellationToken);

            if (existingDish != null)
            {
                return Result.Failure<DishDto>(DishErrors.AlreadyExists(request.Name));
            }

            // Validar precio
            if (request.Price <= 0)
            {
                return Result.Failure<DishDto>(DishErrors.InvalidPrice());
            }

            // Validar nombre
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 3)
            {
                return Result.Failure<DishDto>(DishErrors.InvalidName());
            }

            var dish = new Dish
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Category = (DishCategory)request.Category,
                IsAvailable = request.IsAvailable,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdDish = await _dishRepository.Create(dish, cancellationToken);

            return Result.Success(_mapper.Map<DishDto>(createdDish));
        }

        public async Task<Result<DishDto>> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByID(request.Id, cancellationToken);
            if (dish == null)
            {
                return Result.Failure<DishDto>(DishErrors.NotFound(request.Id));
            }

            // Validar que el nombre no exista (excepto para el mismo plato)
            var existingDish = await _dishRepository.Find(
                d => d.Name.ToLower() == request.Name.ToLower() && d.Id != request.Id,
                cancellationToken);

            if (existingDish != null)
            {
                return Result.Failure<DishDto>(DishErrors.AlreadyExists(request.Name));
            }

            // Validar precio
            if (request.Price <= 0)
            {
                return Result.Failure<DishDto>(DishErrors.InvalidPrice());
            }

            // Validar nombre
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 3)
            {
                return Result.Failure<DishDto>(DishErrors.InvalidName());
            }

            dish.Name = request.Name;
            dish.Description = request.Description;
            dish.Price = request.Price;
            dish.Category = (DishCategory)request.Category;
            dish.IsAvailable = request.IsAvailable;
            dish.UpdatedAt = DateTime.UtcNow;

            await _dishRepository.Update(dish, cancellationToken);

            return Result.Success(_mapper.Map<DishDto>(dish));
        }

        public async Task<Result> Handle(DeleteDishCommand request, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByID(request.Id, cancellationToken);
            if (dish == null)
            {
                return Result.Failure(DishErrors.NotFound(request.Id));
            }

            // Verificar que no tenga pedidos asociados
            var hasOrders = await _orderItemRepository.Find(
                oi => oi.DishId == request.Id,
                cancellationToken);

            if (hasOrders != null)
            {
                return Result.Failure(DishErrors.CannotDeleteWithOrders());
            }

            await _dishRepository.Delete(request.Id, cancellationToken);
            return Result.Success();
        }

        public async Task<Result<DishDto>> Handle(ToggleDishAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByID(request.Id, cancellationToken);
            if (dish == null)
            {
                return Result.Failure<DishDto>(DishErrors.NotFound(request.Id));
            }

            dish.IsAvailable = !dish.IsAvailable;
            dish.UpdatedAt = DateTime.UtcNow;

            await _dishRepository.Update(dish, cancellationToken);

            return Result.Success(_mapper.Map<DishDto>(dish));
        }
    }
}
