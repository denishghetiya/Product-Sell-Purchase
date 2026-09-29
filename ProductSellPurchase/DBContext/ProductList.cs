using System;
using System.Collections.Generic;

namespace ProductSellPurchase.DBContext;

public partial class ProductList
{
    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifyByNavigation { get; set; }

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public virtual ICollection<SellDetail> SellDetails { get; set; } = new List<SellDetail>();

    public virtual User User { get; set; } = null!;
}
