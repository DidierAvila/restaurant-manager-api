using MediatR;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.Dishes.Commands
{
    public class DeleteDishCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
