using Microsoft.AspNetCore.Identity;

namespace Job_Web.Models;
public class ApplicationUser:IdentityUser
{
    public string Name { get; set; }
    public string? Adress { get; set; }
    public string? City { get; set; }
}