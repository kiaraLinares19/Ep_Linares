using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Ep_Linares.Data;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Caching.StackExchangeRedis; 
using Microsoft.Extensions.Logging; // <-- AGREGADO: Necesario para usar ILogger en el catch

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

// --- INICIO: Bloque de Migración de Base de Datos (LA SOLUCIÓN CLAVE) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // El ApplicationDbContext debe ser accesible aquí
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Esto crea el archivo DB si no existe, y aplica todas las migraciones pendientes.
        context.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        // Se asegura de que si hay un error de DB, se muestre en los logs de Render
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al aplicar las migraciones de la base de datos. ¡Revisa tu conexión o las migraciones!");
    }
}
// --- FIN: Bloque de Migración de Base de Datos ---


// Pipeline de Solicitudes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // En tu Dockerfile configuraste ASPNETCORE_ENVIRONMENT=Development
    // Si la aplicación arranca en Development, aquí es donde podrías usar
    // app.UseDeveloperExceptionPage(); si estuvieras en un entorno local,
    // pero en producción/Render, el UseExceptionHandler es más seguro.
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