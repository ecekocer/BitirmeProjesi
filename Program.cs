using BitirmeProjesi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// MSSQL veritaban� ba�lant�s�
builder.Services.AddDbContext<BitirmeProjesiiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionsql")));

// Identity ayarlar�
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<BitirmeProjesiiContext>()
    .AddDefaultTokenProviders();

// Cookie kimlik do�rulama ayarlar�
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";  // Giri� sayfas� yolu
    options.LogoutPath = "/Account/Logout";  // ��k�� sayfas� yolu
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
app.UseAuthentication(); // Kullan�c� kimlik do�rulama middleware'i
app.UseAuthorization();  // Kullan�c� yetkilendirme middleware'i

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
