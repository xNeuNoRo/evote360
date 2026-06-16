using eVote360_Pro.Application;
using eVote360_Pro.Infrastructure;
using eVote360_Pro.WebApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Servicios
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Config de Sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseGlobalExceptionMiddleware();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Activa el uso de sesiones en la aplicación
app.UseSession();

// Mantenemos Authorization para filtros si es necesario
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Ejecutar el Seed de la base de datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<eVote360_Pro.Infrastructure.Contexts.AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<eVote360_Pro.Domain.Interfaces.Security.IPasswordHasher>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    
    await eVote360_Pro.Infrastructure.Persistence.DbSeeder.SeedAdminUserAsync(context, passwordHasher, configuration);
}

app.Run();
