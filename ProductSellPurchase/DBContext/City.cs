using System;
using System.Collections.Generic;

namespace ProductSellPurchase.DBContext;

public partial class City
{
    public int CityId { get; set; }

    public int StateId { get; set; }

    public string CityName { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual State State { get; set; } = null!;
}
