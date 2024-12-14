using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Chọn vai trò
        [HttpGet]
        public IActionResult SelectRole()
        {
            var roles = _roleManager.Roles
                .Where(role => role.Name != "Admin") // Loại bỏ vai trò "Admin"
                .ToList();

            return View(roles); // Trả về danh sách các vai trò, không bao gồm "Admin"
        }

        // Hiển thị danh sách người dùng theo vai trò
        [HttpGet]
        public async Task<IActionResult> UsersByRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Vai trò không hợp lệ.");
            }

            var users = await _userManager.GetUsersInRoleAsync(roleName);
            ViewBag.RoleName = roleName;
            return View(users); // Trả về danh sách người dùng trong vai trò đã chọn
        }

        // Hiển thị form thay đổi mật khẩu
        [HttpGet]
        public async Task<IActionResult> UpdatePassword(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            ViewBag.UserId = userId;
            ViewBag.UserName = user.UserName;
            ViewBag.RoleName = roleName;
            return View(); // Trả về form thay đổi mật khẩu
        }

        // Xử lý thay đổi mật khẩu
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string userId, string newPassword, string confirmPassword, string roleName)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("", "Mật khẩu không được để trống.");
                ViewBag.UserId = userId;
                ViewBag.RoleName = roleName;
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu không khớp.");
                ViewBag.UserId = userId;
                ViewBag.RoleName = roleName;
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Mật khẩu của người dùng {user.UserName} đã được cập nhật thành công.";
                return RedirectToAction("UsersByRole", new { roleName });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewBag.UserId = userId;
            ViewBag.RoleName = roleName;
            return View();
        }
    }
}
