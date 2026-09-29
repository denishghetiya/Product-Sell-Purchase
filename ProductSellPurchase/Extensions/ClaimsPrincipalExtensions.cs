using System.Security.Claims;
using System.Text.Json;
using ProductSellPurchase.DBContext; 

namespace ProductSellPurchase.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static UserPermission GetPagePermission(this ClaimsPrincipal user, int pageId)
        {
            var permissionsJson = user.FindFirst("Permissions")?.Value;

            var allPermissions = JsonSerializer.Deserialize<List<UserPermission>>(permissionsJson);
            return allPermissions?.FirstOrDefault(p => p.PageId == pageId);
        }
    }
}
