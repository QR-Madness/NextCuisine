using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NextCuisine.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NextCuisineContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NextCuisineContext") ?? throw new InvalidOperationException("Connection string 'NextCuisineContext' not found.")));

// Add services to the container.
builder.Services.AddMvc();
builder.Services.AddControllersWithViews();

// Configuration authentication sessions
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "uid";
    options.LoginPath = "/guests/login";
});

builder.Services.AddDistributedMemoryCache();
// Configure the user session
builder.Services.AddSession(options =>
{
    // Set a short timeout for easy testing.
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
