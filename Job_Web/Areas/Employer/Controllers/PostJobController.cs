using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Web.Data;  
using Job_Web.Models; 

namespace YourNamespace.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = "Employer")]
    public class PostJobController : Controller
    {
        private readonly ApplicationDBContext _context;  
        private readonly UserManager<IdentityUser> _userManager;

        public PostJobController(ApplicationDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employer/PostJob/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employer/PostJob/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Job job)
        {
            if (ModelState.IsValid)
            {
                job.PostedDate = DateTime.Now;  // Đặt ngày đăng công việc
                job.EmployerId = User.Identity.Name;  // Đảm bảo EmployerId được gán đúng
                _context.Add(job);  // Thêm công việc vào cơ sở dữ liệu
                await _context.SaveChangesAsync();  // Lưu công việc vào cơ sở dữ liệu
                return RedirectToAction(nameof(Index));  // Chuyển hướng về trang danh sách công việc
            }
            return View(job);  // Nếu có lỗi, giữ lại form và hiển thị thông báo lỗi
        }
        // GET: Employer/PostJob/Index
        public async Task<IActionResult> Index()
        {
            var jobs = await _context.Jobs
                .Where(j => j.EmployerId == User.Identity.Name)  // Lọc công việc theo EmployerId
                .ToListAsync();
            return View(jobs);  // Trả về view với danh sách công việc của Employer
        }
        // GET: Employer/PostJob/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // POST: Employer/PostJob/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);  // Cập nhật công việc
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));  // Quay lại danh sách công việc
            }
            return View(job);
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
        // GET: Employer/PostJob/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Employer/PostJob/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            _context.Jobs.Remove(job);  // Xóa công việc
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));  // Quay lại danh sách công việc
        }


    }
}