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
public async Task<IActionResult> Edit(ApplicationUser model, IFormFile? resumeFile)
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

    // Cập nhật các thông tin khác
    applicationUser.Name = model.Name;
    applicationUser.Email = model.Email;
    applicationUser.Adress = model.Adress;
    applicationUser.City = model.City;
    applicationUser.Education = model.Education;
    applicationUser.WorkExperience = model.WorkExperience;
    applicationUser.Skills = model.Skills;
    applicationUser.DateOfBirth = model.DateOfBirth;

    // Nếu có file CV mới, xử lý việc thay đổi file CV
    if (resumeFile?.Length > 0)
    {
        // Nếu có file CV cũ, xóa file CV cũ
        if (!string.IsNullOrEmpty(applicationUser.ResumeFilePath))
        {
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", applicationUser.ResumeFilePath.TrimStart('/'));
            if (System.IO.File.Exists(oldFilePath))
            {
                try
                {
                    // Xóa file CV cũ
                    System.IO.File.Delete(oldFilePath);
                    _logger.LogInformation("File CV cũ đã bị xóa: {FilePath}", oldFilePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Không thể xóa file CV cũ: {ErrorMessage}", ex.Message);
                    ModelState.AddModelError(string.Empty, "Không thể xóa file CV cũ.");
                    return View(model); // Nếu có lỗi khi xóa file cũ, quay lại trang Edit
                }
            }
        }

        // Đường dẫn lưu trữ file CV mới
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        // Tạo thư mục nếu chưa có
        if (!Directory.Exists(uploadDir))
        {
            Directory.CreateDirectory(uploadDir);
        }

        // Đặt tên file và lưu trữ
        var filePath = Path.Combine(uploadDir, resumeFile.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await resumeFile.CopyToAsync(stream);
        }

        // Cập nhật đường dẫn file CV mới
        applicationUser.ResumeFilePath = $"/uploads/{resumeFile.FileName}";
    }

    // Cập nhật thông tin người dùng qua UserManager
    var result = await _userManager.UpdateAsync(applicationUser);
    if (!result.Succeeded)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model); // Nếu có lỗi, quay lại trang Edit
    }

    _logger.LogInformation("Thông tin người dùng đã được cập nhật thành công.");
    return RedirectToAction(nameof(Show)); // Sau khi cập nhật thành công, chuyển đến trang Show
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
                try
                {
                    // Xóa file
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation("Hồ sơ đã bị xóa: {FilePath}", filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Không thể xóa file hồ sơ: {ErrorMessage}", ex.Message);
                    ModelState.AddModelError(string.Empty, "Không thể xóa file hồ sơ.");
                    return View("Edit", applicationUser); // Quay lại trang chỉnh sửa nếu có lỗi khi xóa file
                }
            }
            else
            {
                _logger.LogWarning("Không tìm thấy file hồ sơ tại: {FilePath}", filePath);
                ModelState.AddModelError(string.Empty, "Không tìm thấy file hồ sơ.");
                return View("Edit", applicationUser); // Quay lại trang chỉnh sửa nếu không tìm thấy file
            }

            // Cập nhật lại đường dẫn file trong cơ sở dữ liệu
            applicationUser.ResumeFilePath = null;

            // Lưu thay đổi qua UserManager
            var result = await _userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Lỗi khi cập nhật thông tin người dùng: {Error}", error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Edit", applicationUser); // Nếu có lỗi, vẫn ở lại trang Edit
            }

            _logger.LogInformation("Thông tin người dùng đã được cập nhật sau khi xóa hồ sơ.");
        }

        // Sau khi xóa, chuyển hướng về trang Show để cập nhật lại giao diện
        return View("Edit", applicationUser);
    }
}