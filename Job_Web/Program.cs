using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Job_Web.Data;
using Job_Web.Models;
using Job_Web.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MyConnection")
                       ?? throw new InvalidOperationException("Connection string 'MyConnection' not found.");

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    // Cấu hình yêu cầu mật khẩu
    options.Password.RequireDigit = false; // Không yêu cầu số
    options.Password.RequiredLength = 6;   // Độ dài tối thiểu là 6 ký tự
    options.Password.RequireNonAlphanumeric = false; // Không yêu cầu ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không yêu cầu chữ in hoa
    options.Password.RequireLowercase = false; // Không yêu cầu chữ thường
}).AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});




var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Admin}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();