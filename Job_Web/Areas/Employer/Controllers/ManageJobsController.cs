using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Web.Data;  // Thay thế với namespace thực tế của bạn
using Job_Web.Models;  // Thay thế với namespace thực tế của bạn

namespace Job_Web.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = "Employer")]
    public class ManageJobsController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ManageJobsController(ApplicationDBContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // GET: Employer/ManageJobs
        public async Task<IActionResult> Index()
        {
            var employerId = _userManager.GetUserId(User);
            var jobs = await _dbContext.Jobs.Where(j => j.EmployerId == employerId).ToListAsync();
            return View(jobs);
        }
    }
}