using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Application.Features.Dishes.Commands;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.Dishes.Handlers
{
    public class DishCommandHandler :
        IRequestHandler<CreateDishCommand, DishDto>,
        IRequestHandler<UpdateDishCommand, DishDto>,
        IRequestHandler<DeleteDishCommand, bool>,
        IRequestHandler<ToggleDishAvailabilityCommand, DishDto>
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

        public async Task<DishDto> Handle(CreateDishCommand request, CancellationToken cancellationToken)
        {
            // Validar que el nombre no exista
            var existingDish = await _dishRepository.Find(
                d => d.Name.ToLower() == request.Name.ToLower(),
                cancellationToken);

            if (existingDish != null)
            {
                throw new Exception($"Ya existe un plato con el nombre '{request.Name}'");
            }

            // Validar precio
            if (request.Price <= 0)
            {
                throw new Exception("El precio debe ser un número mayor a 0");
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

            return _mapper.Map<DishDto>(createdDish);
        }

        public async Task<DishDto> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByID(request.Id, cancellationToken);
            if (dish == null)
            {
                throw new Exception($"Plato con ID {request.Id} no encontrado");
            }

            // Validar que el nombre no exista (excepto para el mismo plato)
            var existingDish = await _dishRepository.Find(
                d => d.Name.ToLower() == request.Name.ToLower() && d.Id != request.Id,
                cancellationToken);

            if (existingDish != null)
            {
                throw new Exception($"Ya existe un plato con el nombre '{request.Name}'");
            }

            // Validar precio
            if (request.Price <= 0)
            {
                throw new Exception("El precio debe ser un número mayor a 0");
            }

            dish.Name = request.Name;
            dish.Description = request.Description;
            dish.Price = request.Price;
            dish.Category = (DishCategory)request.Category;
            dish.IsAvailable = request.IsAvailable;
            dish.UpdatedAt = DateTime.UtcNow;

            await _dishRepository.Update(dish, cancellationToken);

            return _mapper.Map<DishDto>(dish);
        }

        public async Task<bool> Handle(DeleteDishCommand request, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByID(request.Id, cancellationToken);
            if (dish == null)
            {
                throw new Exception($"Plato con ID {request.Id} no encontrado");
            }

            // Verificar que no tenga pedidos asociados
            var hasOrders = await _orderItemRepository.Find(
                oi => oi.DishId == request.Id,
                cancellationToken);

            if (hasOrders != null)
            {
                throw new Exception("No se puede eliminar: este plato tiene pedidos asociados. Desactívalo en su lugar.");
            }

            await _dishRepository.Delete(request.Id, cancellationToken);
            return true;
        }

        public async Task<DishDto> Handle(ToggleDishAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var dish = await _dishRepository.GetByID(request.Id, cancellationToken);
            if (dish == null)
            {
                throw new Exception($"Plato con ID {request.Id} no encontrado");
            }

            dish.IsAvailable = !dish.IsAvailable;
            dish.UpdatedAt = DateTime.UtcNow;

            await _dishRepository.Update(dish, cancellationToken);

            return _mapper.Map<DishDto>(dish);
        }
    }
}
