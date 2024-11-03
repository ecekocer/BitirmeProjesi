using BitirmeProjesi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// MSSQL veritabaný baðlantýsý
builder.Services.AddDbContext<BitirmeProjesiiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionsql")));

// Identity ayarlarý
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<BitirmeProjesiiContext>()
    .AddDefaultTokenProviders();

// Cookie kimlik doðrulama ayarlarý
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";  // Giriþ sayfasý yolu
    options.LogoutPath = "/Account/Logout";  // Çýkýþ sayfasý yolu
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity middleware
app.UseAuthentication(); // Kullanýcý kimlik doðrulama middleware'i
app.UseAuthorization();  // Kullanýcý yetkilendirme middleware'i

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
