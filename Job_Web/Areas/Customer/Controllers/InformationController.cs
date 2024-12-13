using Microsoft.AspNetCore.Mvc;
using Job_Web.Models;
using System.Diagnostics;
using Job_Web.Data;
using Microsoft.AspNetCore.Identity;

namespace Job_Web.Areas.Customer.Controllers;

[Area("Customer")]
public class InformationController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<InformationController> _logger;

    public InformationController(ApplicationDBContext context, UserManager<IdentityUser> userManager, ILogger<InformationController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
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
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        // Lấy ID người dùng từ UserManager
        var customerId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(customerId))
        {
            _logger.LogWarning("Không tìm thấy ID người dùng.");
            return Unauthorized("Bạn cần đăng nhập để truy cập.");
        }

        // Lấy thông tin người dùng dưới dạng IdentityUser
        var userDetails = await _userManager.FindByIdAsync(customerId);
        if (userDetails == null)
        {
            _logger.LogError("Không tìm thấy thông tin người dùng với ID: {CustomerId}", customerId);
            return NotFound("Không tìm thấy thông tin người dùng.");
        }

        // Chuyển đổi sang ApplicationUser
        var applicationUser = userDetails as ApplicationUser;
        if (applicationUser == null)
        {
            _logger.LogError("Không thể ép kiểu IdentityUser thành ApplicationUser.");
            return NotFound("Không thể ép kiểu thành ApplicationUser.");
        }

        // Truyền ApplicationUser vào View để hiển thị form
        return View(applicationUser);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ApplicationUser model, IFormFile resumeFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Lấy thông tin người dùng hiện tại
        var customerId = _userManager.GetUserId(User);
        var userDetails = await _userManager.FindByIdAsync(customerId);

        if (userDetails == null)
        {
            _logger.LogError("Không tìm thấy thông tin người dùng với ID: {CustomerId}", customerId);
            return NotFound("Không tìm thấy thông tin người dùng.");
        }

        // Cập nhật thông tin người dùng
        var applicationUser = userDetails as ApplicationUser;
        if (applicationUser == null)
        {
            _logger.LogError("Không thể ép kiểu IdentityUser thành ApplicationUser.");
            return NotFound("Không thể ép kiểu thành ApplicationUser.");
        }

        applicationUser.Name = model.Name;
        applicationUser.Email = model.Email;
        applicationUser.Adress = model.Adress;
        applicationUser.City = model.City;
        applicationUser.CompanyName = model.CompanyName;
        applicationUser.Education = model.Education;
        applicationUser.WorkExperience = model.WorkExperience;
        applicationUser.Skills = model.Skills;
        applicationUser.DateOfBirth = model.DateOfBirth;
        
        if (resumeFile != null && resumeFile.Length > 0)
        {
            // Đường dẫn nơi lưu trữ file
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Đặt tên file và đường dẫn lưu trữ
            var filePath = Path.Combine(uploadDir, resumeFile.FileName);

            // Lưu file vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await resumeFile.CopyToAsync(stream);
            }

            // Cập nhật đường dẫn file vào ResumeFilePath
            applicationUser.ResumeFilePath = $"/uploads/{resumeFile.FileName}";
        }
        

        // Cập nhật thông tin qua UserManager
        var result = await _userManager.UpdateAsync(applicationUser);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        _logger.LogInformation("Thông tin người dùng đã được cập nhật thành công.");
        return RedirectToAction(nameof(Show)); // Quay lại trang hiển thị thông tin sau khi cập nhật thành công
    }
    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteResume()
{
    // Lấy ID người dùng từ UserManager
    var customerId = _userManager.GetUserId(User);
    if (string.IsNullOrEmpty(customerId))
    {
        _logger.LogWarning("Không tìm thấy ID người dùng.");
        return Unauthorized("Bạn cần đăng nhập để truy cập.");
    }

    // Lấy thông tin người dùng từ UserManager
    var userDetails = await _userManager.FindByIdAsync(customerId);
    if (userDetails == null)
    {
        _logger.LogError("Không tìm thấy thông tin người dùng với ID: {CustomerId}", customerId);
        return NotFound("Không tìm thấy thông tin người dùng.");
    }

    // Chuyển sang ApplicationUser
    var applicationUser = userDetails as ApplicationUser;
    if (applicationUser == null)
    {
        _logger.LogError("Không thể ép kiểu IdentityUser thành ApplicationUser.");
        return NotFound("Không thể ép kiểu thành ApplicationUser.");
    }

    // Kiểm tra nếu người dùng có file hồ sơ và xóa nó
    if (!string.IsNullOrEmpty(applicationUser.ResumeFilePath))
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", applicationUser.ResumeFilePath.TrimStart('/'));

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            _logger.LogInformation("Hồ sơ đã bị xóa.");
        }

        // Cập nhật lại đường dẫn file trong cơ sở dữ liệu
        applicationUser.ResumeFilePath = null;

        // Lưu thay đổi qua UserManager
        var result = await _userManager.UpdateAsync(applicationUser);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("Edit", applicationUser); // Nếu có lỗi, vẫn ở lại trang Edit
        }

        _logger.LogInformation("Thông tin người dùng đã được cập nhật sau khi xóa hồ sơ.");
    }

    // Sau khi xóa, chuyển hướng về trang Show để cập nhật lại giao diện
    return RedirectToAction(nameof(Show));
}

}