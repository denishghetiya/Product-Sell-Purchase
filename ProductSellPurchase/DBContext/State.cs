using System;
using System.Collections.Generic;

namespace ProductSellPurchase.DBContext;

public partial class State
{
    public int StateId { get; set; }

    public int CountryId { get; set; }

    public string StateName { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country Country { get; set; } = null!;
}
