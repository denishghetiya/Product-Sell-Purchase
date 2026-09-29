using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProductSellPurchase.ViewModels
{
    public class CreateUserViewModel
    {
        public int UserId { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsActive { get; set; }
        public IFormFile? Image { get; set; }
        public string? ExistingImagePath { get; set; }
        public List<UserTypeViewModel> UserTypes { get; set; } = new List<UserTypeViewModel>();
    }
    public class UserTypeViewModel
    {
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
        public bool IsActive { get; set; }
        public List<PageViewModel> Pages { get; set; } = new List<PageViewModel>();
    }
    public class PageViewModel
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public bool IsActive { get; set; }
        public List<PermissionTypeViewModel> PermissionType { get; set; } = new List<PermissionTypeViewModel>();
    }
    public class PermissionTypeViewModel
    {
        public int PermissionTypeId { get; set; }
        public string PermissionTypeName { get; set; }
        public bool Permission { get; set; }
    }
}
