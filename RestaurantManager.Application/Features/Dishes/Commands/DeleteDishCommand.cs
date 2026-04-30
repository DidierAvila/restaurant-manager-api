using MediatR;

namespace RestaurantManager.Application.Features.Dishes.Commands
{
    public class DeleteDishCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
