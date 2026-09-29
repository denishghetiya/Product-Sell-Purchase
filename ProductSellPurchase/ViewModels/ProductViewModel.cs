using System.ComponentModel.DataAnnotations;

namespace ProductSellPurchase.ViewModels
{
    public class CreateProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
    
}
