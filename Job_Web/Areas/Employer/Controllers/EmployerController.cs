using Microsoft.AspNetCore.Mvc;

namespace Job_Web.Areas.Employer.Controllers
{
    [Area("Employer")]
    public class EmployerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}