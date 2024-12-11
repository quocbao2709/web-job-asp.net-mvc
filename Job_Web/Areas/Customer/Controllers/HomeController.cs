using Microsoft.AspNetCore.Mvc;
using Job_Web.Models;
using System.Diagnostics;
using Job_Web.Data;
using Microsoft.AspNetCore.Identity;

namespace Job_Web.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDBContext context, UserManager<IdentityUser> userManager, ILogger<HomeController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
    public async Task<IActionResult> Show()
    {
        // Lấy ID người dùng từ UserManager
        var customerId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(customerId))
        {
            _logger.LogWarning("Không tìm thấy ID người dùng.");
            return Unauthorized("Bạn cần đăng nhập để truy cập.");
        }

        _logger.LogInformation("ID người dùng hiện tại: {CustomerId}", customerId);

        // Lấy thông tin người dùng dưới dạng IdentityUser
        var userDetails = await _userManager.FindByIdAsync(customerId);

        if (userDetails == null)
        {
            _logger.LogError("Không tìm thấy thông tin người dùng với ID: {CustomerId}", customerId);
            return NotFound("Không tìm thấy thông tin người dùng.");
        }

        // Chuyển đổi từ IdentityUser sang ApplicationUser (ép kiểu)
        var applicationUser = userDetails as ApplicationUser;

        // Kiểm tra nếu ép kiểu thành công
        if (applicationUser == null)
        {
            _logger.LogError("Không thể ép kiểu IdentityUser thành ApplicationUser.");
            return NotFound("Không thể ép kiểu thành ApplicationUser.");
        }

        // Truyền thông tin người dùng vào View
        return View(applicationUser);  // Truyền ApplicationUser vào View
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}