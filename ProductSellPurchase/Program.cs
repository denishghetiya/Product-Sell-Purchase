using Microsoft.EntityFrameworkCore;
using ProductSellPurchase.DBContext;
using ProductSellPurchase.Helper;
using ProductSellPurchase.Middleware;
using ProductSellPurchase.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ProductSellPurchaseDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IJwtHelper, JwtHelper>();
builder.Services.AddSingleton<EncryptionHelper>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login/Login";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login/AccessDenied";
        options.Cookie.Name = "AuthCookie";
    });

builder.Services.AddAuthorization();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseNoCache();
app.UseSession();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
