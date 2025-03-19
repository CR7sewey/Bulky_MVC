using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();


//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // add dependcy injection to the implementation!
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // add dependcy injection to the implementation!
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// stripe Injection keys
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// binds the IndentityUser to the EntityFramework
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options => // middleware for authentication and authorization, override the default path
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// facebook auth
builder.Services.AddAuthentication().AddFacebook(options => // https://developers.facebook.com/apps/643041038325503/settings/basic/
{
    options.AppId = "643041038325503";
    options.AppSecret = "09baa3ae98d5dbc0dd4915a5e7627ae8";
});


// adding session to the services
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// email sender
builder.Services.AddScoped<IEmailSender, EmailSender>();

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
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"]; // get the secret key from the appsettings.json
app.UseRouting();
app.UseAuthentication(); // if username or password is valid - middleware
app.UseAuthorization(); // acess based on role - ex Admin and Customer

app.UseSession();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"); // Customer is the default area

app.Run();
