using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrazWMS.Data;
using OrazWMS.Models;

var builder = WebApplication.CreateBuilder(args);

//Konfiguracja bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//POPRAWIONA REJESTRACJA IDENTITY (zamiast IdentityUser -> ApplicationUser)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;    // Musi zawiera� cyfr�
    options.Password.RequiredLength = 8;     // Minimalna d�ugo�� 8 znak�w
    options.Password.RequireUppercase = true; // Musi zawiera� wielk� liter�
    options.Password.RequireLowercase = true; // Musi zawiera� ma�� liter�
    options.Password.RequireNonAlphanumeric = true; // Musi zawiera� znak specjalny
}).AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//DODANIE OBS�UGI UserManager I RoleManager DO DI
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//Inicjalizacja r�l i u�ytkownika Admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await EnsureRolesAsync(roleManager);
    await InitializeAdminAsync(userManager, roleManager);
}

//Konfiguracja �rodowiska
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Mapowanie tras
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

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
            Console.WriteLine($"Rola {roleName} zosta�a dodana do bazy.");
        }
    }
}

/// <summary>
/// Inicjalizuje u�ytkownika Admin, je�li nie istnieje.
/// </summary>
async Task InitializeAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    const string adminEmail = "admin@example.com";
    const string adminPassword = "Admin123!";
    const string adminRole = "Admin";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine("U�ytkownik Admin zosta� pomy�lnie utworzony.");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"{error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("U�ytkownik Admin ju� istnieje.");
    }
}
