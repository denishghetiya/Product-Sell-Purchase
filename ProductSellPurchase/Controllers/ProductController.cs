using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductSellPurchase.DBContext;
using ProductSellPurchase.ViewModels;

namespace ProductSellPurchase.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ProductSellPurchaseDBContext _context;

        public ProductController(ProductSellPurchaseDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> ProductList() 
        { 
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductList([FromBody] RequestPaginationViewModel model)
        {
            var query = _context.ProductLists.Include(u=>u.CreatedByNavigation).Include(u=>u.ModifyByNavigation)
                .Where(u => u.IsDeleted != true)
                .AsQueryable();

            var totalCount = query.Count();

            var Order = query
                .Skip(model.Start)
                .Take(model.Length)
                .Select(a => new CreateProductViewModel
                {
                    ProductId = a.ProductId,
                    ProductName = a.ProductName,
                    Price = a.Price,
                    CreatedBy = a.CreatedByNavigation.Username,
                    CreatedDate = a.CreatedDate,
                    ModifyBy = a.ModifyByNavigation.Username,
                    ModifyDate = a.ModifyDate,
                }).OrderByDescending(a => a.ProductId).ToList();

            return Json(new
            {
                draw = model.Draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = Order
            });
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct(int? productId)
        {
            if(productId > 0)
            {
                var product = await _context.ProductLists.FirstOrDefaultAsync(f => f.ProductId == productId && f.IsDeleted != true);
                
                var model = new CreateProductViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price
                };
                return View(model);
            }
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel model)
        {
            if (model.ProductId == 0) {
                int userIdd = int.Parse(User.FindFirst("UserId").Value);
                var product = new ProductList
                {
                    ProductName = model.ProductName,
                    Price = model.Price,
                    CreatedBy = userIdd,
                    CreatedDate = DateTime.Now,
                    ModifyBy = null,
                    ModifyDate = null,
                };
                _context.ProductLists.Add(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Product created successfully." });
            }
            if(model.ProductId > 0)
            {
                int userIdd = int.Parse(User.FindFirst("UserId").Value);
                var product = await _context.ProductLists.FirstOrDefaultAsync(f => f.ProductId == model.ProductId && f.IsDeleted != true);
                if (product == null) return Json(new { success = false, message = "Product Not Found." });

                product.ProductName = model.ProductName;
                product.Price = model.Price;
                product.ModifyBy = userIdd;
                product.ModifyDate = DateTime.Now;

                _context.ProductLists.Update(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Product updated successfully." });
            }
            return Json(new { success = false, message = "Something went wrong." });
        }
        
        public async Task<IActionResult> DeleteProduct(int? productId)
        {
            var product = await _context.ProductLists.FirstOrDefaultAsync(f => f.ProductId == productId && f.IsDeleted != true);
            product.IsDeleted = true;
            _context.ProductLists.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductList");
        }
    }
}




