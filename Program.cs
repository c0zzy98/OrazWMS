using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrazWMS.Data;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Konfiguracja Identity (u¿ytkownicy, role, logowanie)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Wymaga potwierdzenia konta
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

// Uruchomienie aplikacji
app.Run();

/// <summary>
/// Inicjalizacja u¿ytkownika Admin oraz ról.
/// </summary>
/// <param name="app">Aplikacja WebApplication.</param>
async Task InitializeAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await InitializeAdminUserAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        // Logowanie b³êdów
        Console.WriteLine($"B³¹d podczas inicjalizacji bazy danych: {ex.Message}");
    }
}

/// <summary>
/// Tworzy u¿ytkownika Admin oraz przypisuje role, jeœli nie istniej¹.
/// </summary>
/// <param name="userManager">Menad¿er u¿ytkowników.</param>
/// <param name="roleManager">Menad¿er ról.</param>
async Task InitializeAdminUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    const string adminEmail = "admin@example.com";
    const string adminPassword = "Admin123!";
    const string adminRole = "Admin";

    // Tworzenie roli Admin, jeœli jeszcze nie istnieje
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Sprawdzenie, czy u¿ytkownik Admin ju¿ istnieje
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
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
            Console.WriteLine("B³¹d podczas tworzenia u¿ytkownika Admin:");
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
