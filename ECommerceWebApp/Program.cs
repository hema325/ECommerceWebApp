using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using DataAccess.DataAccessRepository.Repository;
using ECommerceWebApp.Services.BackGroundServices;
using ECommerceWebApp.Constrains;
using ECommerceWebApp.Hubs;
using ECommerceWebApp.Options;
using ECommerceWebApp.Seeding;
using ECommerceWebApp.Services.Email;
using Microsoft.AspNetCore.Identity;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddOptions<SmtpConfigurationOption>().Bind(builder.Configuration.GetSection(SmtpConfigurationOption.SmtpConfiguration)).ValidateDataAnnotations();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(provider => new UnitOfWork(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IDBInitializer, DBInitializer>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = Schemes.Default;
})
.AddCookie(Schemes.Default, options =>
{
    options.LoginPath = "/Home/Index";
    options.LogoutPath = "/Home/Index";
    options.AccessDeniedPath = "/Identity/SignInOut/AccessDenied";
})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration.GetSection("Authentication:Facebook")["AppId"];
    options.AppSecret = builder.Configuration.GetSection("Authentication:Facebook")["AppSecret"];
    options.SaveTokens = true;
});
builder.Services.AddSignalR();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});
builder.Services.AddHostedService<TokensCleanUpHostedService>();

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

app.MapHub<UserHub>("/Hubs/User");
app.UseSession();

app.MapControllerRoute(
  name: "areas",
  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

StripeConfiguration.ApiKey = app.Configuration.GetSection("Stripe")["SecretKey"];

await SeedAsync();

app.Run();


async Task SeedAsync()
{
     await app.Services.CreateScope().ServiceProvider.GetRequiredService<IDBInitializer>().SeedAsync();
}