using System;
using System.Collections.Generic;

namespace Webapp.Models.GravityBookstore;

public partial class ShippingMethod
{
    public int MethodId { get; set; }

    public string? MethodName { get; set; }

    public double? Cost { get; set; }

    public virtual ICollection<CustOrder> CustOrders { get; set; } = new List<CustOrder>();
}
