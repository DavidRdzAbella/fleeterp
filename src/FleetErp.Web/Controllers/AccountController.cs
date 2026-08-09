using FleetErp.Web.Models;
using FleetErp.Web.Services;
using FleetErp.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetErp.Web.Controllers;

/// <summary>Entrada y salida del portal.</summary>
[AllowAnonymous]
public sealed class AccountController(IFleetApiClient api, ISessionContext session) : Controller
{
    [HttpGet("/acceso")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginForm());
    }

    [HttpPost("/acceso")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginForm form, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(form);

        try
        {
            var login = await api.LoginAsync(new LoginRequest(form.TenantSlug, form.Email, form.Password));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                session.BuildPrincipal(login),
                new AuthenticationProperties
                {
                    IsPersistent = form.RememberMe,
                    ExpiresUtc = login.ExpiresAtUtc
                });

            return Redirect(SafeRedirect(returnUrl));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty,
                ex.IsUnauthorized ? "Empresa, correo o contraseña incorrectos." : ex.UserMessage);
            return View(form);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty,
                "No hay conexión con el servicio de flotilla. Verifique que la API esté encendida.");
            return View(form);
        }
    }

    [HttpPost("/salir")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet("/acceso/denegado")]
    public IActionResult Denied() => View();

    [HttpGet("/error")]
    public IActionResult Error() => View();

    /// <summary>Solo se acepta un retorno local: evita usar el login como trampolín a otro sitio.</summary>
    private string SafeRedirect(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
}

