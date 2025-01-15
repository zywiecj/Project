using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using Webapp.Models.GravityBookstore;
using Webapp.Models.Movies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Webapp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
       

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddMemoryCache();
        builder.Services.AddSession();
        builder.Services.AddControllersWithViews();

        // Register MoviesDbContext
        builder.Services.AddDbContext<MoviesDbContext>(op =>
        {
            try
            {
                op.UseSqlite(builder.Configuration["MoviesDatabase:ConnectionString"]);
            }
            catch (SqliteException)
            {
                op.UseSqlite(builder.Configuration["MoviesDatabase:ConnectionStringUpperCase"]);
            }
        });


// For AppDbContext
        builder.Services.AddDbContext<AppDbContext>(op =>
        {
            try
            {
                op.UseSqlite(builder.Configuration["AccountDatabase:ConnectionString"]);
            }
            catch (SqliteException)
            {
                op.UseSqlite(builder.Configuration["AccountDatabase:ConnectionStringUpperCase"]);
            }
        });

// For SuperheroesContext
        builder.Services.AddDbContext<GravityBookstoreDBContext>(options =>
        {
            try
            {
                options.UseSqlite(builder.Configuration["GravityBookstoreDatabase:ConnectionString"]);
            }
            catch (SqliteException)
            {
                options.UseSqlite(builder.Configuration["GravityBookstoreDatabase:ConnectionStringUpperCase"]);
            }
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
        
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        // Register IEmailSender service
        builder.Services.AddTransient<IEmailSender, EmailSender>();
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
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.MapRazorPages();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implement your email sending logic here
            return Task.CompletedTask;
        }
    }
}