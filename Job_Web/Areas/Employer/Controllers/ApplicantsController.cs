using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Web.Data;  // Thay thế với namespace thực tế của bạn
using Job_Web.Models;  // Thay thế với namespace thực tế của bạn

namespace Job_Web.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = "Employer")]
    public class ApplicantsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ApplicantsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Employer/Applicants/Index/5
        public async Task<IActionResult> Index(int jobId)
        {
            var job = await _context.Jobs
                .Include(j => j.Applications)
                .ThenInclude(a => a.Customer)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Employer/Applicants/Shortlist/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Shortlist(int jobId, string applicantId)
        {
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.CustomerId == applicantId);

            if (application != null)
            {
                application.Status = "Shortlisted"; // Đánh dấu ứng viên được chọn
                _context.Update(application);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { jobId });
        }
    }

}