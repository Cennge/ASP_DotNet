using CenngeShop.Data;
using CenngeShop.Middleware.Auth.Session;
using CenngeShop.Middleware.Auth.Token;
using CenngeShop.Middleware.Cart;
using CenngeShop.Middleware.Demo;
using CenngeShop.Middleware.Ticks;
using CenngeShop.Services.DateTime;
using CenngeShop.Services.Hash;
using CenngeShop.Services.Kdf;
using CenngeShop.Services.Scoped;
using CenngeShop.Services.Storage;
using CenngeShop.Services.Transient;
using Microsoft.EntityFrameworkCore;

// PostgreSQL (Supabase): map DateTime -> 'timestamp without time zone'
// so existing Unspecified-kind dates (seed data, CreatedAt, Birthdate) work as on SQL Server.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddHash();
builder.Services.AddKdf();
builder.Services.AddDateTime();
builder.Services.AddStorage();

builder.Services.AddScoped<ScopedService>();
builder.Services.AddTransient<TransientService>();


builder.Services.AddDistributedMemoryCache();          // Налаштування сесій
builder.Services.AddSession(options =>                 // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state
{                                                      // 
    options.IdleTimeout = TimeSpan.FromMinutes(10);    // 
    options.Cookie.HttpOnly = true;                    // 
    options.Cookie.IsEssential = true;                 // 
});

builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("MainDb")));

builder.Services.AddScoped<DataAccessor>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("LocalTesting", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();
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
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapStaticAssets();

// Місце для обробників користувача (Custom Middleware)
app.UseSession();
app.UseDemo();
app.UseTicks();
app.UseAuthSession();
app.UseAuthToken();
app.UseCart();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
