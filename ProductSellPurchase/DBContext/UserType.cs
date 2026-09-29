using System;
using System.Collections.Generic;

namespace ProductSellPurchase.DBContext;

public partial class UserType
{
    public int UserTypeId { get; set; }

    public string UserTypeName { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
