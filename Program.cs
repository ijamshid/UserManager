
using UserManager.Application;
using UserManager.Infrastructure;
using UserManager.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);



var configuration = builder.Configuration;

builder.Services.AddDistributedMemoryCache();

// Session uchun
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // zarur bo’lsa, boshqa opsiyalar...
});

builder.Services
    .AddDatabase(configuration)
    .AddApplication()
    .AddJwtAuthentication(configuration)
    .AddConfigurations(configuration)
    .AddSessions();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

