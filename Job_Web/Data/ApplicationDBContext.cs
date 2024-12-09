using Job_Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Job_Web.Models;
using Microsoft.AspNetCore.Identity;

namespace Job_Web.Data;

public class ApplicationDBContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }
    
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<JobCategory> JobCategories { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình quan hệ giữa Job và JobCategory
        modelBuilder.Entity<Job>()
            .HasOne(j => j.Employer)
            .WithMany()  // Một nhà tuyển dụng có thể có nhiều công việc
            .HasForeignKey(j => j.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);   // Tránh việc xóa cascade từ JobCategory
    }
    
}

