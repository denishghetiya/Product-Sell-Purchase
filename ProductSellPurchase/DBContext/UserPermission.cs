using System;
using System.Collections.Generic;

namespace ProductSellPurchase.DBContext;

public partial class UserPermission
{
    public int UserPermissionId { get; set; }

    public int UserTypeId { get; set; }

    public int PageId { get; set; }

    public int PermissionTypeId { get; set; }

    public bool Permission { get; set; }

    public bool IsDeleted { get; set; }

    public virtual PageName Page { get; set; } = null!;

    public virtual PermissionType PermissionType { get; set; } = null!;

    public virtual UserType UserType { get; set; } = null!;
}
