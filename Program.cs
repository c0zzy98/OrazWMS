using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrazWMS.Data;
using OrazWMS.Models;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Konfiguracja Identity (U¯YJ ApplicationUser zamiast IdentityUser!)
builder.Services.AddIdentity<OrazWMS.Models.ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Konfiguracja kontrolerów i widoków
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ?? Inicjalizacja ról i u¿ytkownika Admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<OrazWMS.Models.ApplicationUser>>();

    await EnsureRolesAsync(roleManager); // ?? Teraz role bêd¹ inicjalizowane zawsze
    await InitializeAdminAsync(userManager, roleManager);
}

// Konfiguracja œrodowisk
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Obs³uga logowania u¿ytkownika
app.UseAuthorization();  // Obs³uga autoryzacji ról

// Mapowanie tras
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Dodanie trasy do UsersController
app.MapControllerRoute(
    name: "users",
    pattern: "Users/{action=Index}/{id?}",
    defaults: new { controller = "Users" });

app.Run();

/// <summary>
/// Inicjalizuje role Admin i User w bazie danych.
/// </summary>
async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($" Rola {roleName} zosta³a dodana do bazy.");
        }
    }
}

/// <summary>
/// Inicjalizuje u¿ytkownika Admin, jeœli nie istnieje.
/// </summary>
async Task InitializeAdminAsync(UserManager<OrazWMS.Models.ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    const string adminEmail = "admin@example.com";
    const string adminPassword = "Admin123!";
    const string adminRole = "Admin";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new OrazWMS.Models.ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine(" U¿ytkownik Admin zosta³ pomyœlnie utworzony.");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($" {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine(" U¿ytkownik Admin ju¿ istnieje.");
    }
}
