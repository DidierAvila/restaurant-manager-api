using System;
using System.Collections.Generic;

namespace RestaurantManager.Api.ScaffoldedModels;

public partial class Order
{
    public int Id { get; set; }

    public int TableNumber { get; set; }

    public string WaiterName { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
