﻿using System;
using System.Collections.Generic;

namespace Webapp.Models.GravityBookstore;

public partial class CustomerAddress
{
    public int CustomerId { get; set; }

    public int AddressId { get; set; }

    public int? StatusId { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
