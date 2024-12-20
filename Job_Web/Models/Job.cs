using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Job_Web.Models;

public class Job
{
    public int Id { get; set; } // ID công việc
    [Required]
    public string Title { get; set; } // Tiêu đề công việc
    [Required]
    public string Description { get; set; } // Mô tả công việc
    [Required]
    public string Location { get; set; } // Địa điểm công việc
    [Required]
    [Range(0, double.MaxValue)]
    public double Salary { get; set; } // Lương công việc
    public DateTime PostedDate { get; set; } = DateTime.Now; // Ngày đăng công việc
    [Required]
    public string? EmployerId { get; set; } // ID của nhà tuyển dụng
    [ForeignKey(nameof(EmployerId))]
    [ValidateNever]
    public ApplicationUser Employer { get; set; } // Mối quan hệ với nhà tuyển dụng
    [Required]
    [ValidateNever]
    public int JobCategoryId { get; set; } // ID ngành nghề
    [ForeignKey(nameof(JobCategoryId))]
    [ValidateNever]
    public JobCategory JobCategory { get; set; } // Liên kết ngành nghề
}