using RestaurantManager.Domain.Common;

namespace RestaurantManager.Domain.Errors;

public static class OrderErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Order.NotFound",
        $"El pedido con ID {id} no fue encontrado");

    public static Error OrderItemNotFound(int id) => Error.NotFound(
        "Order.OrderItemNotFound",
        $"El item de pedido con ID {id} no fue encontrado");

    public static Error TableAlreadyHasActiveOrder(int tableNumber) => Error.Conflict(
        "Order.TableAlreadyHasActiveOrder",
        $"La mesa {tableNumber} ya tiene un pedido activo");

    public static Error InvalidTableNumber() => Error.Validation(
        "Order.InvalidTableNumber",
        "El número de mesa debe ser mayor a 0");

    public static Error InvalidWaiter() => Error.Validation(
        "Order.InvalidWaiter",
        "El nombre del mesero es requerido");

    public static Error CannotEditClosedOrder() => Error.Conflict(
        "Order.CannotEditClosedOrder",
        "No se puede editar un pedido cerrado");

    public static Error CannotDeleteNonOpenOrder() => Error.Conflict(
        "Order.CannotDeleteNonOpenOrder",
        "Solo se pueden eliminar pedidos en estado 'Abierto'");

    public static Error CannotAdvanceStatus() => Error.Conflict(
        "Order.CannotAdvanceStatus",
        "No se puede avanzar el estado del pedido");

    public static Error OrderHasNoItems() => Error.Validation(
        "Order.OrderHasNoItems",
        "El pedido debe tener al menos un plato");

    public static Error DishNotAvailable(string dishName) => Error.Conflict(
        "Order.DishNotAvailable",
        $"El plato '{dishName}' no está disponible");

    public static Error InvalidQuantity() => Error.Validation(
        "Order.InvalidQuantity",
        "La cantidad debe ser mayor a 0");
}
