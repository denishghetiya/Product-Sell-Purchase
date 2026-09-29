using ProductSellPurchase.DBContext;
using System.ComponentModel.DataAnnotations;

namespace ProductSellPurchase.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
    }
    public class RegisterUserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public IFormFile Image { get; set; }
    }
    public class ResetPasswordViewModel
    {
        public string? Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class UserInfoViewModel
    {
        public int UserId { get; set; }
        public int UserTypeId { get; set; }
        public string Username { get; set; }
        public string UserTypeName { get; set; }
        public string Email { get; set; }
    }
    public class EditProfileViewModel
    {
        public int? UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public IFormFile? Image { get; set; }
        public string? ExistingImagePath { get; set; }
    }
    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class PermissionViewModel
    {
        public int UserPermissionId { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
        public int PageId { get; set; }
        public string PageName1 { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public int PermissionTypeId { get; set; }
        public string PermissionTypeName { get; set; }
        public bool Permission { get; set; }
    }
}
