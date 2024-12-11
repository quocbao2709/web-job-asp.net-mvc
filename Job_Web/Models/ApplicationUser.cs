using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace Job_Web.Models;
public class ApplicationUser:IdentityUser
{
    public string Name { get; set; }
    public string? Adress { get; set; }
    public string? City { get; set; }
    public string? CompanyName { get; set; }
    [Required]
    public bool IsApproved { get; set; }
    public string? Education { get; set; } // Trình độ học vấn
    public string? WorkExperience { get; set; } // Kinh nghiệm làm việc
    public string? ResumeFilePath { get; set; } // Đường dẫn đến file hồ sơ xin việc
    public string? Skills { get; set; } // Các kỹ năng chuyên môn
    public DateTime? DateOfBirth { get; set; } // Ngày sinh (nếu cần thiết)
    
}