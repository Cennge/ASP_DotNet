using asp_all.Services.Hash;
using asp_all.Middleware.Demo;
using asp_all.Services.Scoped;
using asp_all.Services.Transient;
using asp_all.Services.Kdf;
using asp_all.Data;
using Microsoft.EntityFrameworkCore;
using asp_all.Services.DateTime;
using asp_all.Middleware.Ticks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddHash();
builder.Services.AddKdf();
builder.Services.AddDateTime();

builder.Services.AddScoped<ScopedService>();
builder.Services.AddTransient<TransientService>();


builder.Services.AddDistributedMemoryCache();          // Налаштування сесій
builder.Services.AddSession(options =>                 // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state
{                                                      // 
    options.IdleTimeout = TimeSpan.FromSeconds(10);    // 
    options.Cookie.HttpOnly = true;                    // 
    options.Cookie.IsEssential = true;                 // 
});

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MainDb")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

// Місце для обробників користувача (Custom Middleware)
app.UseSession();
app.UseDemo();
app.UseTicks();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
