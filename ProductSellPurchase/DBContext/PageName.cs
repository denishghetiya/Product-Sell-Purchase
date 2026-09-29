using System;
using System.Collections.Generic;

namespace ProductSellPurchase.DBContext;

public partial class PageName
{
    public int PageId { get; set; }

    public string PageName1 { get; set; } = null!;

    public string Controller { get; set; } = null!;

    public string Action { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
