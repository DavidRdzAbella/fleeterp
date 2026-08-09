using System.Globalization;
using FleetErp.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Sesión y propagación del token hacia la API.
builder.Services.AddScoped<ISessionContext, SessionContext>();
builder.Services.AddTransient<ApiTokenHandler>();

// El portal es un cliente más de la API: solo conoce su URL base, nunca su base
// de datos. Esto es lo que permite desplegar y escalar ambos por separado.
var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Configure Api:BaseUrl con la dirección de FleetErp.Api.");

builder.Services.AddHttpClient<IFleetApiClient, FleetApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl.EndsWith('/') ? apiBaseUrl : apiBaseUrl + "/");
    client.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue("Api:TimeoutSeconds", 30));
}).AddHttpMessageHandler<ApiTokenHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/acceso";
        options.LogoutPath = "/salir";
        options.AccessDeniedPath = "/acceso/denegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "fleeterp.session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Todo el portal habla español de México: fechas, moneda y separadores deben
// verse como los espera el usuario sin formatear a mano en cada vista.
var culture = new CultureInfo(builder.Configuration["Localization:Culture"] ?? "es-MX");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture),
    SupportedCultures = [culture],
    SupportedUICultures = [culture]
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
