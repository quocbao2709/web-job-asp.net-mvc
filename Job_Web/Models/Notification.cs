using System;

namespace Job_Web.Models;
public class Notification
{
    public int Id { get; set; } // ID thông báo
    public string UserId { get; set; } // ID người dùng nhận thông báo
    public string Message { get; set; } // Nội dung thông báo
    public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày giờ thông báo được tạo
    public bool IsRead { get; set; } = false; // Trạng thái đọc (đã đọc hay chưa)
        
    // Navigation property: Tham chiếu đến người dùng nhận thông báo
    public ApplicationUser User { get; set; } 
}