using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using ProductSellPurchase.DBContext;
using ProductSellPurchase.Helper;
using ProductSellPurchase.ViewModels;

namespace ProductSellPurchase.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ProductSellPurchaseDBContext _context;
        private readonly IWebHostEnvironment _webHost;
        private readonly EncryptionHelper _encryptionHelper;

        public DashboardController(ProductSellPurchaseDBContext context, IWebHostEnvironment webHost, EncryptionHelper encryptionHelper)
        {
            _context = context;
            _webHost = webHost;
            _encryptionHelper = encryptionHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile(int? userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(f => f.UserId == userId && f.IsActive == true && f.IsDeleted != true);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                //Password = _encryptionHelper.Decrypt(user.Password),
                ExistingImagePath = user.ImageName != null ? $"/Uploads/{user.ImageName}" : null
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(f => f.UserId == model.UserId && f.IsActive == true && f.IsDeleted != true);
            if (user == null) return Json(new { success = false, message = "User Not Found." });

            var email = _context.Users.FirstOrDefault(u =>u.UserId != model.UserId && u.Email == model.Email && u.IsDeleted != true);
            if (email != null)
            {
                return Json(new { success = false, message = "This Email has already account." });
            }

            if (email == null)
            {
                user.Email = model.Email;
            }

            user.Username = model.Username;
            //user.Password = _encryptionHelper.Encrypt(model.Password);

            if (model.Image != null)
            {
                var uploadsFolder = Path.Combine(_webHost.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(user.ImageName))
                {
                    var oldImagePath = Path.Combine(uploadsFolder, user.ImageName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var originalFileName = Path.GetFileNameWithoutExtension(model.Image.FileName);
                var extension = Path.GetExtension(model.Image.FileName);
                var uniqueFileName = $"{originalFileName}_{timeStamp}{extension}";

                var fileSavePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                user.ImageName = uniqueFileName;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "User Updated Successfully." });
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                return Json(new { success = false, message = "New Password and Confirm Password should be same." });
            }
            int userId = int.Parse(User.FindFirstValue("UserID"));
            var user = await _context.Users.FirstOrDefaultAsync(f => f.UserId == userId);
            if (user == null) return Json(new { success = false, message = "User Not Found." });
            if (model.OldPassword != _encryptionHelper.Decrypt(user.Password))
            {
                return Json(new { success = false, message = "Old Password is incorrect." });
            }
            if (model.NewPassword == _encryptionHelper.Decrypt(user.Password))
            {
                return Json(new { success = false, message = "New Password should not be same as Last Old Password." });
            }
            user.Password = _encryptionHelper.Encrypt(model.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Password Changed Successfully." });
        }
    }
}


