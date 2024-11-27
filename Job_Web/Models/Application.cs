using Microsoft.AspNetCore.Identity;

namespace Job_Web.Models;
public class Application
{
    public int Id { get; set; }  // ID của đơn ứng tuyển
    public string CustomerId { get; set; }  // ID của ứng viên (Customer)
    public int JobId { get; set; }  // ID của công việc mà ứng viên nộp đơn vào
    public string Status { get; set; }  // Trạng thái của đơn ứng tuyển (Shortlisted, Rejected, etc.)

    // Navigation property: Mỗi Application có một Job (công việc)
    public Job Job { get; set; }

    // Navigation property: Mỗi Application có một Customer (ứng viên)
    public IdentityUser Customer { get; set; }
}