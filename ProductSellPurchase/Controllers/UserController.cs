using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductSellPurchase.DBContext;
using ProductSellPurchase.Extensions;
using ProductSellPurchase.Helper;
using ProductSellPurchase.Services;
using ProductSellPurchase.ViewModels;

namespace ProductSellPurchase.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ProductSellPurchaseDBContext _context;
        private readonly EmailService _emailService;
        private readonly EncryptionHelper _encryptionHelper;
        private readonly IWebHostEnvironment _webHost;

        public UserController(ProductSellPurchaseDBContext context, EmailService emailService, EncryptionHelper encryptionHelper, IWebHostEnvironment webHost)
        {
            _context = context;
            _emailService = emailService;
            _encryptionHelper = encryptionHelper;
            _webHost = webHost;
        }
        [CheckAccess(1, 4)]
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            return View();
        }
        [CheckAccess(1, 4)]
        [HttpPost]
        public async Task<IActionResult> UserList([FromBody] RequestPaginationViewModel model)
        {
            var query = _context.Users.Include(u=>u.UserType)
                .Where(u => u.IsDeleted != true)
                .AsQueryable();

            var totalCount = query.Count();

            var Order = query
                .Skip(model.Start)
                .Take(model.Length)
                .Select(a => new CreateUserViewModel
                {
                    UserId = a.UserId,
                    UserTypeId = a.UserTypeId,
                    UserTypeName = a.UserType.UserTypeName,
                    Username = a.Username,
                    Email = a.Email,
                    IsActive = a.IsActive,
                }).OrderByDescending(a => a.UserId).ToList();

            return Json(new
            {
                draw = model.Draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = Order
            });
        }
        [CheckAccess(1, 1)]
        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var roles = await _context.UserTypes
                .Where(r=>r.IsActive == true && r.IsDeleted != true)
                .Select(r => new UserTypeViewModel
                {
                    UserTypeId = r.UserTypeId, 
                    UserTypeName = r.UserTypeName
                })
                .ToListAsync();

            var model = new CreateUserViewModel
            {
                UserTypes = roles
            };

            return View(model);
        }
        [CheckAccess(1, 1)]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return Json(new { success = false, message = "Password and ConfirmPassword are should be same." });
            }
            var email = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.IsDeleted != true);
            if (email == null)
            {
                if (model.Image != null)
                {
                    var uploadsFolder = Path.Combine(_webHost.WebRootPath, "Uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
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

                    var user = new User
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = _encryptionHelper.Encrypt(model.Password),
                        UserTypeId = model.UserTypeId,
                        CreatedDate = DateTime.Now,
                        ImageName = uniqueFileName,
                        IsActive = true
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    var resetLink = Url.Action("ActiveAccount", "Login", new { email = model.Email }, Request.Scheme);
                    string subject = "Confirm the Request";
                    string body = $@"<!DOCTYPE html>
                        <html>
                        <body>
                            <p>Click the button below to Confirm the request:</p>
                            <a href=""{resetLink}"">Confirm</a>
                        </body>
                        </html>";

                    //await _emailService.SendEmailAsync(model.Email, subject, body);
                    //return Json(new { success = true, message = "Register Successfully. Go in your Email Account and confirm the request." });
                    return Json(new { success = true, message = "User Created Successfully." });
                }
                return Json(new { success = false, message = "Image not found." });
            }
            return Json(new { success = false, message = "This Email has already account." });
        }
        [CheckAccess(1, 2)]
        [HttpGet]
        public async Task<IActionResult> EditUser(int userId)
        {
            var user = await _context.Users.Include(u=>u.UserType).FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted != true);
            if (user == null) return NotFound();

            var roles = await _context.UserTypes
                .Where(r => r.IsDeleted != true)
                .Select(r => new UserTypeViewModel
                {
                    UserTypeId = r.UserTypeId,
                    UserTypeName = r.UserTypeName
                })
                .ToListAsync();

            var model = new CreateUserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Password = _encryptionHelper.Decrypt(user.Password),
                ConfirmPassword = _encryptionHelper.Decrypt(user.Password),
                UserTypeId = user.UserTypeId,
                UserTypeName = user.UserType.UserTypeName,
                IsActive = user.IsActive,
                ExistingImagePath = user.ImageName != null ? $"/Uploads/{user.ImageName}" : null,
                UserTypes = roles
            };

            return View(model);
        }
        [CheckAccess(1, 2)]
        [HttpPost]
        public async Task<IActionResult> EditUser(CreateUserViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(f => f.UserId == model.UserId && f.IsActive == true && f.IsDeleted != true);
            if (user == null) return Json(new { success = false, message = "User Not Found." });

            var email = _context.Users.FirstOrDefault(u => u.UserId != model.UserId && u.Email == model.Email && u.IsDeleted != true);
            if (email != null)
            {
                return Json(new { success = false, message = "This Email has already account." });
            }

            if (email == null)
            {
                user.Email = model.Email;
            }
            user.UserTypeId = model.UserTypeId;
            user.Username = model.Username;
            user.Password = _encryptionHelper.Encrypt(model.Password);

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

            //int userIdd = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            //if (userIdd == model.UserId) return RedirectToAction("Logout", "Login");

            return Json(new { success = true, message = "User Updated Successfully." });
        }
        [CheckAccess(1, 2)]
        [HttpPost]
        public async Task<IActionResult> ChangeUserStatus(int userId, bool isActive)
        {
            var userstatus = _context.Users.FirstOrDefault(a => a.UserId == userId && a.IsDeleted != true);
            userstatus.IsActive = isActive;
            _context.Users.Update(userstatus);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "User Status Changed Successfully." });
        }
        [CheckAccess(1, 3)]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted != true);
            if (user == null) return Json(new { success = false, message = "User Not Found." });
            user.IsActive = false;
            user.IsDeleted = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "User Deleted Successfully." });
        }

    }
}
