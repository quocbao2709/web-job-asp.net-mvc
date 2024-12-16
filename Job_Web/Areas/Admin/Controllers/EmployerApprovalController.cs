using Job_Web.Data;
using Job_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmployerApprovalController : Controller
    {
        private readonly ApplicationDBContext _context;

        public EmployerApprovalController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy tất cả người dùng chưa được phê duyệt và có vai trò là "Employer"
            var employers = await _context.ApplicationUsers
                .Where(u => u.IsApproved == false)
                .Join(_context.UserRoles, 
                    user => user.Id, 
                    role => role.UserId, 
                    (user, role) => new { user, role })
                .Where(joined => _context.Roles.Any(r => r.Id == joined.role.RoleId && r.Name == "Employer"))
                .Select(joined => joined.user)
                .ToListAsync();

            return View(employers);
        }
        

        [HttpPost]
        public async Task<IActionResult> ApproveEmployer(string userId)
        {
            var user = await _context.ApplicationUsers.FindAsync(userId);

            if (user != null)
            {
                user.IsApproved = true; // Phê duyệt tài khoản Employer
                _context.ApplicationUsers.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Điều hướng về danh sách
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Reject(string userId)
        {
            var user = await _context.ApplicationUsers.FindAsync(userId);

            if (user != null)
            {
                _context.ApplicationUsers.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
