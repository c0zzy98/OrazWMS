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

// Inicjalizacja u¿ytkownika Admin i ról
await InitializeAdminAsync(app);

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
/// Inicjalizacja u¿ytkownika Admin oraz ról.
/// </summary>
async Task InitializeAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<OrazWMS.Models.ApplicationUser>>(); // POPRAWKA: U¿yj ApplicationUser
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await InitializeAdminUserAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"B³¹d podczas inicjalizacji bazy danych: {ex.Message}");
    }
}

/// <summary>
/// Tworzy u¿ytkownika Admin oraz przypisuje role, jeœli nie istniej¹.
/// </summary>
async Task InitializeAdminUserAsync(UserManager<OrazWMS.Models.ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    const string adminEmail = "admin@example.com";
    const string adminPassword = "Admin123!";
    const string adminRole = "Admin";

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new OrazWMS.Models.ApplicationUser // POPRAWKA: U¿yj ApplicationUser zamiast IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine("U¿ytkownik Admin zosta³ pomyœlnie utworzony.");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("U¿ytkownik Admin ju¿ istnieje.");
    }
}
