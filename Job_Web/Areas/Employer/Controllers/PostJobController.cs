using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Web.Data;  
using Job_Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Job_Web.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = "Employer")]
    public class PostJobController : Controller
    {
        private readonly ApplicationDBContext _context;

        public PostJobController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Employer/PostJob/Create
        public IActionResult Create()
        {
            // Lấy danh sách các JobCategory từ cơ sở dữ liệu
            var jobCategories = _context.JobCategories.ToList();

            // Truyền danh sách vào ViewBag để hiển thị trong dropdown list
            ViewBag.JobCategories = new SelectList(jobCategories, "Id", "Profession");

            return View();
        }

        // POST: Employer/PostJob/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Job job)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin nhà tuyển dụng từ ApplicationUser (dựa trên User.Identity.Name)
                var employer = _context.ApplicationUsers
                    .FirstOrDefault(u => u.UserName == User.Identity.Name);

                if (employer != null)
                {
                    // Cập nhật thông tin employer cho công việc
                    job.EmployerId = employer.Id;

                    // Lưu công việc mới vào cơ sở dữ liệu
                    _context.Add(job);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // Redirect về trang danh sách công việc sau khi tạo
                }

                // Nếu không tìm thấy nhà tuyển dụng, trả về lỗi
                ModelState.AddModelError(string.Empty, "Employer not found.");
            }

            // Nếu có lỗi, trả về lại View
            var jobCategories = _context.JobCategories.ToList();
            ViewBag.JobCategories = new SelectList(jobCategories, "Id", "Profession", job.JobCategoryId);
            return View(job);
        }
        // GET: Employer/PostJob/Index
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin nhà tuyển dụng từ ApplicationUsers
            var employer = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (employer == null)
            {
                return Unauthorized(); // Trả về lỗi nếu không tìm thấy user
            }

            var jobs = await _context.Jobs
                .Where(j => j.EmployerId == employer.Id)
                .ToListAsync();

            return View(jobs);
        }
        // GET: Employer/PostJob/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Lấy thông tin nhà tuyển dụng từ ApplicationUsers
            var employer = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (employer == null)
            {
                return Unauthorized(); // Không tìm thấy nhà tuyển dụng
            }

            // Tìm công việc và kiểm tra quyền sở hữu
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == employer.Id);

            if (job == null)
            {
                return NotFound(); // Công việc không tồn tại hoặc không thuộc về user hiện tại
            }

            // Lấy danh sách JobCategory cho dropdown
            ViewBag.JobCategories = await _context.JobCategories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Profession
                })
                .ToListAsync();

            return View(job); // Trả về View với dữ liệu công việc
        }


        // POST: Employer/PostJob/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Job job)
        {
            // Xác nhận Id của công việc cần chỉnh sửa
            if (id != job.Id)
            {
                return NotFound();
            }

            // Lấy thông tin nhà tuyển dụng từ ApplicationUsers
            var employer = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (employer == null)
            {
                return Unauthorized(); // Không tìm thấy nhà tuyển dụng
            }

            // Kiểm tra quyền sở hữu
            var existingJob = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == employer.Id);

            if (existingJob == null)
            {
                return NotFound(); // Công việc không tồn tại hoặc không thuộc về user hiện tại
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật dữ liệu
                    existingJob.Title = job.Title;
                    existingJob.Description = job.Description;
                    existingJob.Location = job.Location;
                    existingJob.Salary = job.Salary;
                    existingJob.JobCategoryId = job.JobCategoryId;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.Update(existingJob);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index)); // Quay lại danh sách công việc
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Jobs.Any(j => j.Id == job.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            // Lấy lại danh sách JobCategory nếu model không hợp lệ
            ViewBag.JobCategories = await _context.JobCategories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Profession
                })
                .ToListAsync();

            return View(job); // Trả về lại View để sửa lỗi
        }
        
        // GET: Employer/PostJob/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // Lấy thông tin nhà tuyển dụng từ ApplicationUsers
            var employer = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (employer == null)
            {
                return Unauthorized(); // Trả về lỗi nếu không tìm thấy user
            }

            // Tìm công việc dựa trên Id và kiểm tra quyền sở hữu
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == employer.Id);

            if (job == null)
            {
                return NotFound(); // Công việc không tồn tại hoặc không thuộc về user hiện tại
            }

            return View(job); // Hiển thị thông tin công việc để xác nhận xóa
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