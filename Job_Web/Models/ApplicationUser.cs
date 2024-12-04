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
    
}