using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using ProductSellPurchase.DBContext;

namespace ProductSellPurchase.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CheckAccessAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int _pageId;
        private readonly int _permissionType;

        public CheckAccessAttribute(int pageId, int permissionType)
        {
            _pageId = pageId;
            _permissionType = permissionType;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Login", null);
                return;
            }

            var permissionsJson = user.FindFirst("Permissions")?.Value;

            if (string.IsNullOrEmpty(permissionsJson))
            {
                context.Result = new StatusCodeResult(403);
                return;
            }

            var permissions = JsonSerializer.Deserialize<List<UserPermission>>(permissionsJson);
            var permission = permissions?.FirstOrDefault(p => p.PageId == _pageId && p.PermissionTypeId == _permissionType);

            if (permission == null)
            {
                context.Result = new StatusCodeResult(403);
                return;
            }

            if (permission.Permission != true)
            {
                context.Result = new StatusCodeResult(403);
            }

        }
    }
}
