using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Ep_Linares.Data;
using Microsoft.Extensions.Caching.StackExchangeRedis; 

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        "Data Source=portalacademico.db"));

// Configuración de Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// CONFIGURACIÓN DE REDIS CACHÉ 
var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");

// Nota: Debes añadir la conexión en appsettings.json. Si no existe, este bloque fallará.
if (string.IsNullOrEmpty(redisConnection))
{
    // Esto es un buen punto de fallo si olvidas la configuración.
    throw new InvalidOperationException("La cadena de conexión 'RedisConnection' no está configurada en appsettings.json."); 
}

builder.Services.AddStackExchangeRedisCache(options =>
{
    // Usamos la cadena de conexión de appsettings.json
    options.Configuration = redisConnection;
    options.InstanceName = "EpLinares_"; // Un prefijo para tus claves de caché
});

// CONFIGURACIÓN DE SESIÓN 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline de Solicitudes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
