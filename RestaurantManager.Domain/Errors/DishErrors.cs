using RestaurantManager.Domain.Common;

namespace RestaurantManager.Domain.Errors;

public static class DishErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Dish.NotFound",
        $"El plato con ID {id} no fue encontrado");

    public static Error AlreadyExists(string name) => Error.Conflict(
        "Dish.AlreadyExists",
        $"Ya existe un plato con el nombre '{name}'");

    public static Error InvalidPrice() => Error.Validation(
        "Dish.InvalidPrice",
        "El precio debe ser mayor a 0");

    public static Error InvalidCategory() => Error.Validation(
        "Dish.InvalidCategory",
        "La categoría especificada no es válida");

    public static Error CannotDeleteWithOrders() => Error.Conflict(
        "Dish.CannotDeleteWithOrders",
        "No se puede eliminar el plato porque tiene pedidos asociados");

    public static Error InvalidName() => Error.Validation(
        "Dish.InvalidName",
        "El nombre del plato es requerido y debe tener al menos 3 caracteres");
}
