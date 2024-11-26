using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Job_Web.Models;

namespace Job_Web.Data;

public class ApplicationDBContext : IdentityDbContext
{
    
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }
}

