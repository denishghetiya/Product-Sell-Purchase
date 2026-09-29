using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductSellPurchase.DBContext;
using ProductSellPurchase.ViewModels;
using System.Text.Json;

namespace ProductSellPurchase.Controllers
{
    [Authorize]
    public class SidebarMenuController : Controller
    {
        private readonly ProductSellPurchaseDBContext _context;

        public SidebarMenuController(ProductSellPurchaseDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetSidebarPages()
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (userId == 0) return RedirectToAction("Login", "Account");

            var permissionsJson = User.FindFirst("Permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsJson))
                return Json(new List<object>());

            var permissions = JsonSerializer.Deserialize<List<PermissionViewModel>>(permissionsJson);

            var pageIdsWithShowPermission = permissions
                .Where(p => p.PermissionTypeId == 5 && p.Permission == true )
                .Select(p => p.PageId)
                .Distinct()
                .ToList();

            var pages = permissions
                .Where(p => p.PermissionTypeId == 5 && p.Permission == true)
                .Select(p => new
                {
                    pageId = p.PageId,
                    pageName = p.PageName1,
                    controller = p.Controller,
                    action = p.Action
                })
                .ToList();

            return Json(pages);
        }
    }
}
