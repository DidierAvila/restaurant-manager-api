using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace RestaurantManager.Domain.Entities;

public enum DishCategory
{
    [PgName("Entradas")]
    [Display(Name = "Entradas")]
    Entradas = 1,

    [PgName("PlatosFuertes")]
    [Display(Name = "Platos Fuertes")]
    PlatosFuertes = 2,

    [PgName("Sopas")]
    [Display(Name = "Sopas")]
    Sopas = 3,

    [PgName("Bebidas")]
    [Display(Name = "Bebidas")]
    Bebidas = 4,

    [PgName("Postres")]
    [Display(Name = "Postres")]
    Postres = 5
}
