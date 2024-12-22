using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project_WEB2.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Project_WEB2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Project_WEB2Context") ?? throw new InvalidOperationException("Connection string 'Project_WEB2Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
//----
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(1); });
//----
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
//---
app.UseSession();
//---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=userall}/{action=login}");

app.Run();
