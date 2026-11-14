using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
builder.Services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => { o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; o.IdleTimeout = TimeSpan.FromMinutes(30); });
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ApiClient>(c => c.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5184/"))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false });
var app = builder.Build();
app.UseExceptionHandler("/Home/Error");
if (!app.Environment.IsDevelopment()) app.UseHsts();
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseSession();
app.Use(async (context, next) =>
{
    try { await next(); }
    catch (ApiSessionExpiredException)
    {
        context.Session.Clear();
        context.Response.Redirect("/Account/Login");
    }
});
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
app.MapControllerRoute(name:"default", pattern:"{controller=Home}/{action=Index}/{id?}");
app.Run();
