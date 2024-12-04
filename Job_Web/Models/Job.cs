using System.Net.Mime;
using Job_Web.Models;
using Microsoft.AspNetCore.Identity;

namespace Job_Web.Models;


public class Job
{
    public int Id { get; set; } // ID công việc

    public string Title { get; set; } // Tiêu đề công việc

    public string Description { get; set; } // Mô tả công việc

    public string Location { get; set; } // Địa điểm công việc

    public double Salary { get; set; } // Lương công việc

    public DateTime PostedDate { get; set; } = DateTime.Now; // Ngày đăng công việc

    public string EmployerId { get; set; } // ID nhà tuyển dụng (thực tế là ID của nhà tuyển dụng, không phải model)

    public ApplicationUser Employer { get; set; } // Mối quan hệ giữa Job và Employer (một Job thuộc về một Employer)

    // Navigation property: Một Job có thể có nhiều Application
    public ICollection<Application> Applications { get; set; } // Mỗi Job có thể có nhiều đơn ứng tuyển (Application)
    // Thuộc tính JobCategoryId (khóa ngoại)
    public int JobCategoryId { get; set; }

    // Navigation property đến JobCategory
    public JobCategory JobCategory { get; set; }
}