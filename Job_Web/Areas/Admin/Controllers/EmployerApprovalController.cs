using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Job_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; 

namespace Job_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmployerApprovalController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployerApprovalController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy tất cả người dùng chưa được phê duyệt
            var users = await _userManager.Users
                .Where(u => u.IsApproved == false)
                .ToListAsync();

            // Lọc chỉ giữ những người dùng có role là "Employer"
            var employers = new List<ApplicationUser>();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Employer"))
                {
                    employers.Add(user);
                }
            }

            return View(employers);
        }


        [HttpPost]
        public async Task<IActionResult> ApproveEmployer(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var applicationUser = user as ApplicationUser;

                if (applicationUser != null)
                {
                    applicationUser.IsApproved = true;  // Phê duyệt tài khoản Employer

                    var result = await _userManager.UpdateAsync(applicationUser);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");  // Điều hướng về danh sách
                    }
                }
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Reject(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}