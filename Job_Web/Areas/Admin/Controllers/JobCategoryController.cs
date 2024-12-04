using Microsoft.AspNetCore.Mvc;
using Job_Web.Models;
using Job_Web.Data; // Giả định tên DbContext là ApplicationDbContext
using System.Linq;
using Job_Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Job_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class JobCategoryController : Controller
    {
        private readonly ApplicationDBContext _context;

        public JobCategoryController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.JobCategories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            // Lấy danh sách các JobCategory từ cơ sở dữ liệu
            var jobCategories = _context.JobCategories.ToList();

            // Chuyển đổi danh sách JobCategory thành SelectList
            ViewBag.JobCategories = new SelectList(jobCategories, "Id", "Profession");
            return View();
        }

        [HttpPost]
        public IActionResult Create(JobCategory category)
        {
            if (ModelState.IsValid)
            {
                _context.JobCategories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int id)
        {
            var category = _context.JobCategories.Find(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(JobCategory category)
        {
            if (ModelState.IsValid)
            {
                _context.JobCategories.Update(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = _context.JobCategories.Find(id);
            if (category == null)
                return NotFound();

            _context.JobCategories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}