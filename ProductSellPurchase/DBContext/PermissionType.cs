using System;
using System.Collections.Generic;

namespace ProductSellPurchase.DBContext;

public partial class PermissionType
{
    public int PermissionTypeId { get; set; }

    public string PermissionTypeName { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
