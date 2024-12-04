using System.Collections.Generic;
using Job_Web.Models;

namespace Job_Web.Models;
public class JobCategory
{
    public int Id { get; set; } // ID ngành nghề
    public string Profession { get; set; } // Tên ngành nghề (VD: Công nghệ thông tin, Marketing)
    public string Description { get; set; } // Mô tả ngắn gọn về ngành nghề (tuỳ chọn)

    // Navigation property: Mỗi ngành nghề có thể chứa nhiều công việc
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}