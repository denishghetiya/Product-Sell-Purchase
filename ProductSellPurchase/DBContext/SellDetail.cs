using System;
using System.Collections.Generic;

namespace ProductSellPurchase.DBContext;

public partial class SellDetail
{
    public int SellDetailId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public DateTime SellDate { get; set; }

    public decimal Price { get; set; }

    public decimal Quantity { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ProductList Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
