using System.Security.Claims;
using Job_Web.Data;
using Job_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Job_Web.Areas.Employer.Controllers;

[Area("Employer")]
[Authorize(Roles = "Employer")]
public class PostJobController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<PostJobController> _logger;

    public PostJobController(ApplicationDBContext context, UserManager<IdentityUser> userManager, ILogger<PostJobController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    // Hiển thị form tạo công việc
    public IActionResult Create()
    {
        LoadJobCategories();
        return View();
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Job job)
    {
        if (ModelState.IsValid)
        {
            // Lấy thông tin người dùng hiện tại
           // var currentUser = await _userManager.GetUserAsync(User);

           // if (currentUser == null)
          //  {
            //    _logger.LogWarning("Người dùng hiện tại không tồn tại.");
           //     return Unauthorized();
           // }

            //_logger.LogInformation("Người dùng hiện tại ID: {UserId}", currentUser.Id);

            // Gán EmployerId cho công việc
           // job.EmployerId = currentUser.Id;

            // Nếu JobCategoryId được truyền vào, tìm ngành nghề và gán vào công việc
            var jobCategory = await _context.JobCategories.FindAsync(job.JobCategoryId);
            if (jobCategory != null)
            {
                job.JobCategory = jobCategory;  // Gán ngành nghề cho công việc
            }

            try
            {
                // Lưu công việc vào cơ sở dữ liệu
                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();
               // _logger.LogInformation("Đã lưu công việc thành công với ID: {JobId}", job.Id);
                return RedirectToAction(nameof(Index)); // Điều hướng đến trang danh sách công việc
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu công việc.");
                ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
            }
        }

        // Nếu có lỗi, tải lại danh sách các JobCategories
        LoadJobCategories();
        return View(job);
    }


    // Hiển thị danh sách công việc của nhà tuyển dụng hiện tại
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            _logger.LogWarning("Người dùng hiện tại không tồn tại.");
            return Unauthorized();
        }

        var jobs = await _context.Jobs
            .Where(j => j.EmployerId == currentUser.Id)
            .Include(j => j.JobCategory) // Bao gồm thông tin về JobCategory
            .ToListAsync();

        _logger.LogInformation("Hiển thị danh sách {JobCount} công việc cho người dùng ID: {UserId}", jobs.Count, currentUser.Id);

        return View(jobs);
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var job = await _context.Jobs
            .Include(j => j.JobCategory)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            return NotFound();
        }

        // Kiểm tra quyền sở hữu công việc
        var currentUser = await _userManager.GetUserAsync(User);
        if (job.EmployerId != currentUser.Id)
        {
            return Unauthorized();
        }

        LoadJobCategories();
        return View(job);
    }
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
            // Tìm JobCategory và gán lại
            var jobCategory = await _context.JobCategories.FindAsync(job.JobCategoryId);
            if (jobCategory != null)
            {
                job.JobCategory = jobCategory;
            }

            try
            {
                _context.Update(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Jobs.Any(j => j.Id == job.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        LoadJobCategories();
        return View(job);
    }
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var job = await _context.Jobs
            .Include(j => j.JobCategory)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            return NotFound();
        }

        // Kiểm tra quyền sở hữu công việc
        var currentUser = await _userManager.GetUserAsync(User);
        if (job.EmployerId != currentUser.Id)
        {
            return Unauthorized();
        }

        return View(job);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var job = await _context.Jobs.FindAsync(id);

        if (job == null)
        {
            return NotFound();
        }

        // Kiểm tra quyền sở hữu công việc
        var currentUser = await _userManager.GetUserAsync(User);
        if (job.EmployerId != currentUser.Id)
        {
            return Unauthorized();
        }

        try
        {
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa công việc.");
            ModelState.AddModelError("", "Có lỗi xảy ra khi xóa công việc.");
            return View(job);
        }
    }

    // Hàm tải danh sách JobCategories
    private void LoadJobCategories()
    {
        var jobCategories = _context.JobCategories.ToList();
        ViewBag.JobCategories = new SelectList(jobCategories, "Id", "Profession");
    }
    public async Task<IActionResult> GetApplicants()
    {
        var employerId = _userManager.GetUserId(User);
        var jobs = await _context.Jobs
            .Where(j => j.EmployerId == employerId) 
            .Select(j => j.Id) 
            .ToListAsync(); 
        var applications = await _context.Applications
            .Where(a => jobs.Contains(a.JobId)) 
            .Include(a => a.Job) 
            .Include(a => a.Customer) 
            .ToListAsync(); 
        return View(applications);
    }
}
