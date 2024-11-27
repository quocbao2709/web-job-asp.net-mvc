namespace Job_Web.Models;


public class Employer
{
    public string Id { get; set; }  // ID của nhà tuyển dụng (EmployerId)
    public string Name { get; set; }  // Tên công ty của nhà tuyển dụng
    public string CompanyName { get; set; }  // Tên công ty
    public string Address { get; set; }  // Địa chỉ của công ty
    public string City { get; set; }  // Thành phố của công ty

    // Navigation property: Một Employer có thể có nhiều Jobs
    public ICollection<Job> Jobs { get; set; }
}