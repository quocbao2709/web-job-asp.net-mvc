using System.Security.Claims;
using Job_Web.Data;
using Job_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = "Customer")]
public class JobController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public JobController(ApplicationDBContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> GetListJob()
    {
        var job = await _context.Jobs.Include(j => j.JobCategory).ToListAsync();
        return View(job);
    }
    public async Task<IActionResult> GetDetailJob(int id)
    {
        var job = await _context.Jobs.Include(j => j.JobCategory)
                                      .FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
        {
            return NotFound(); 
        }
        return View(job);
    }
    [HttpPost]
    public async Task<IActionResult> ApplyForJob(int jobId)
    {
        var customerId = _userManager.GetUserId(User);
        var existingApplication = await _context.Applications
            .FirstOrDefaultAsync(a => a.JobId == jobId && a.CustomerId == customerId);
        var user = await _userManager.FindByIdAsync(customerId) as ApplicationUser;
        if (user == null)
        {
            // Nếu không tìm thấy người dùng, có thể trả về lỗi hoặc chuyển hướng
            return RedirectToAction("Error", "Home");
        }

        // Kiểm tra xem thông tin người dùng có đầy đủ không
        if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Adress)
            || string.IsNullOrEmpty(user.City) || string.IsNullOrEmpty(user.Education) || string.IsNullOrEmpty(user.WorkExperience)
            || string.IsNullOrEmpty(user.Skills) || !user.DateOfBirth.HasValue || string.IsNullOrEmpty(user.ResumeFilePath))
        {
            // Nếu thông tin chưa đầy đủ, hiển thị popup yêu cầu cập nhật thông tin
            TempData["Error"] = "Bạn cần cập nhật thông tin cá nhân trước khi nộp đơn.";
            return RedirectToAction("Show", "Information");
        }
        
        if (existingApplication != null)
        {
            return RedirectToAction("AppliedJobs", "Job");
        }
        var application = new Application
        {
            JobId = jobId,
            CustomerId = customerId,
            Status = "Pending" 
        };
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
        return RedirectToAction("AppliedJobs", "Job");
    }
    public async Task<IActionResult> AppliedJobs()
    {
        var customerId = _userManager.GetUserId(User);
        var appliedJobs = await _context.Applications
            .Include(a => a.Job)
            .ThenInclude(j => j.JobCategory)
            .Where(a => a.CustomerId == customerId)
            .Select(a => new
            {
                JobTitle = a.Job.Title,
                Profession = a.Job.JobCategory.Profession,
                Location = a.Job.Location,
                Status = a.Status
            })
            .ToListAsync();
        return View(appliedJobs);
    }

}