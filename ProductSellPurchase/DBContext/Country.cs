using System;
using System.Collections.Generic;

namespace ProductSellPurchase.DBContext;

public partial class Country
{
    public int CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
