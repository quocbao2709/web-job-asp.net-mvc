using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Job_Web.Models;

namespace Job_Web.Data;

public class ApplicationDBContext : IdentityDbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }
    
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Employer> Employers { get; set; }
    public DbSet<Application> Applications { get; set; }
    
    
}

