using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.Domain.Entities.AccessControl;

public enum UserTypeEnum
{
    [Display(Name = "Consultor")]
    Consultant,
    [Display(Name = "Asistente")]
    Assistant,
    [Display(Name = "Proveedor")]
    Supplier,
    [Display(Name = "Cliente")]
    Client
}
