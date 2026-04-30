using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace RestaurantManager.Domain.Entities;

public enum OrderStatus
{
    [PgName("Abierto")]
    [Display(Name = "Abierto")]
    Abierto = 1,

    [PgName("EnPreparacion")]
    [Display(Name = "En Preparación")]
    EnPreparacion = 2,

    [PgName("Listo")]
    [Display(Name = "Listo")]
    Listo = 3,

    [PgName("Entregado")]
    [Display(Name = "Entregado")]
    Entregado = 4,

    [PgName("Cerrado")]
    [Display(Name = "Cerrado")]
    Cerrado = 5,
}
