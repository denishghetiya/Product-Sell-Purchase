using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ProductSellPurchase.DBContext;
using ProductSellPurchase.ViewModels;
using ProductSellPurchase.Extensions;

namespace ProductSellPurchase.Controllers
{
    [Authorize]
    public class UserTypeController : Controller
    {
        private readonly ProductSellPurchaseDBContext _context;

        public UserTypeController(ProductSellPurchaseDBContext context)
        {
            _context = context;
        }
        [CheckAccess(2, 5)]
        [HttpGet]
        public async Task<IActionResult> UserTypeList()
        {
            return View();
        }
        [CheckAccess(2, 5)]
        [HttpPost]
        public async Task<IActionResult> UserTypeList([FromBody] RequestPaginationViewModel model)
        {
            var query = _context.UserTypes.Include(u=>u.UserPermissions)
                .Where(u => u.IsDeleted != true)
                .AsQueryable();

            var totalCount = query.Count();

            var Order = query
                .Skip(model.Start)
                .Take(model.Length)
                .Select(a => new UserTypeViewModel
                {
                    UserTypeId = a.UserTypeId,
                    UserTypeName = a.UserTypeName,
                    IsActive = a.IsActive,
                }).OrderByDescending(a => a.UserTypeId).ToList();

            return Json(new
            {
                draw = model.Draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = Order
            });
        }

        [CheckAccess(2, 1)]
        [HttpGet]
        public async Task<IActionResult> CreateType()
        {
            var permissiontypes = await _context.PermissionTypes
                .Select(r => new PermissionTypeViewModel
                {
                    PermissionTypeId = r.PermissionTypeId,
                    PermissionTypeName = r.PermissionTypeName,
                    //Permission = false
                })
                .ToListAsync();

            var pages = await _context.PageNames
                .Select(r => new PageViewModel
                {
                    PageId = r.PageId,
                    PageName = r.PageName1,
                    //PermissionType = permissiontypes,
                    PermissionType = permissiontypes
                        .Select(pt => new PermissionTypeViewModel
                        {
                            PermissionTypeId = pt.PermissionTypeId,
                            PermissionTypeName = pt.PermissionTypeName,
                            //Permission = false
                        }).ToList()
                })
                .ToListAsync();

            var model = new UserTypeViewModel
            {
                Pages = pages,
            };

            return View(model);
        }
        [CheckAccess(2, 1)]
        [HttpPost]
        public async Task<IActionResult> CreateType(UserTypeViewModel model)
        {
            var usertype = _context.UserTypes.Any(u=>u.UserTypeName == model.UserTypeName && u.IsDeleted != true);
            if (usertype == true)
            {
                return Json(new { success = false, message = "This UserType is already exist." });
            }
            var role = new UserType
            {
                UserTypeName = model.UserTypeName,
                IsActive = model.IsActive,
                UserPermissions = model.Pages.SelectMany(p => p.PermissionType.Select(pt => new UserPermission
                {
                    PageId = p.PageId,
                    PermissionTypeId = pt.PermissionTypeId,
                    Permission = pt.Permission,
                })).ToList()
            };
            _context.UserTypes.Add(role);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "UserType Created Successfully." });
        }
        [CheckAccess(2, 2)]
        [HttpGet]
        public async Task<IActionResult> EditType(int userTypeId)
        {
            var role = await _context.UserTypes
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(f => f.UserTypeId == userTypeId && f.IsDeleted != true);

            if (role == null)
                return NotFound();

            var permissionTypes = await _context.PermissionTypes
                .Where(pt => pt.IsDeleted != true)
                .Select(pt => new PermissionTypeViewModel
                {
                    PermissionTypeId = pt.PermissionTypeId,
                    PermissionTypeName = pt.PermissionTypeName
                })
                .ToListAsync();

            var dbPages = await _context.PageNames
                .Where(p => p.IsDeleted != true)
                .Select(p => new
                {
                    p.PageId,
                    p.PageName1
                })
                .ToListAsync();

            var pages = dbPages.Select(p => new PageViewModel
            {
                PageId = p.PageId,
                PageName = p.PageName1,
            
                PermissionType = permissionTypes
                    .Select(pt => new PermissionTypeViewModel
                    {
                        PermissionTypeId = pt.PermissionTypeId,
                        PermissionTypeName = pt.PermissionTypeName,
            
                        Permission = role.UserPermissions.Any(rp =>
                            rp.PageId == p.PageId &&
                            rp.PermissionTypeId == pt.PermissionTypeId &&
                            rp.Permission == true &&
                            rp.IsDeleted != true)
                    })
                    .ToList()
            })
            .ToList();

            var model = new UserTypeViewModel
            {
                UserTypeId = role.UserTypeId,
                UserTypeName = role.UserTypeName,
                IsActive = role.IsActive,
                Pages = pages
            };

            return View(model);
        }

        [CheckAccess(2, 2)]
        [HttpPost]
        public async Task<IActionResult> EditType(UserTypeViewModel model)
        {
            var role = await _context.UserTypes
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(f => f.UserTypeId == model.UserTypeId && f.IsDeleted != true);

            if (role == null)
                return Json(new { success = false, message = "UserType Not Found." });

            role.UserTypeName = model.UserTypeName;
            role.IsActive = model.IsActive;

            foreach (var page in model.Pages)
            {
                foreach (var perm in page.PermissionType)
                {
                    var existing = role.UserPermissions
                        .FirstOrDefault(up => up.PageId == page.PageId && up.PermissionTypeId == perm.PermissionTypeId);

                    if (perm.Permission) 
                    {
                        if (existing == null)
                        {
                            role.UserPermissions.Add(new UserPermission
                            {
                                UserTypeId = role.UserTypeId,
                                PageId = page.PageId,
                                PermissionTypeId = perm.PermissionTypeId,
                                Permission = true,
                                IsDeleted = false
                            });
                        }
                        else
                        {
                            existing.Permission = true;
                            existing.IsDeleted = false;
                        }
                    }
                    else 
                    {
                        if (existing != null)
                        {
                            existing.Permission = false;
                        }
                    }
                }
            }

            _context.UserTypes.Update(role);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "UserType Updated Successfully." });
        }

        [CheckAccess(2, 2)]
        [HttpPost]
        public async Task<IActionResult> ChangeUserTypeStatus(int userTypeId, bool isActive)
        {
            var usertypestatus = _context.UserTypes.FirstOrDefault(a => a.UserTypeId == userTypeId && a.IsDeleted != true);
            usertypestatus.IsActive = isActive;
            _context.UserTypes.Update(usertypestatus);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "UserType Status Changes Successfully." });
        }
        
        [CheckAccess(2, 3)]
        [HttpPost]
        public async Task<IActionResult> DeleteType(int usertypeid)
        {
            var role = await _context.UserTypes.Include(u => u.UserPermissions).FirstOrDefaultAsync(f=>f.UserTypeId == usertypeid && f.IsDeleted != true);
            if (role == null) return Json(new { success = false, message = "User Not Found." });
            var roleinuse = await _context.Users.AnyAsync(f=>f.UserTypeId == usertypeid && f.IsDeleted != true);
            if (roleinuse == true) return Json(new { success = false, message = "This Type is in Use. Cann't Deleted." });
            role.IsActive = false;
            role.IsDeleted = true;
            foreach (var perm in role.UserPermissions)
            {
                perm.IsDeleted = true;
            }
            _context.UserTypes.Update(role);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "UserType Deleted Successfully." });
        }

    }
}
