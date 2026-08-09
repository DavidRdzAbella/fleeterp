using System.Security.Claims;
using System.Text.Json;
using FleetErp.Web.Models;

namespace FleetErp.Web.Services;

/// <summary>
/// Sesión del portal. El token de la API y la parametrización de la empresa
/// viajan en la cookie de autenticación: así el portal no necesita estado en
/// servidor y puede escalar a varias instancias sin sesión pegajosa.
/// </summary>
public interface ISessionContext
{
    bool IsAuthenticated { get; }
    string? ApiToken { get; }
    string UserName { get; }
    UserRole Role { get; }
    string TenantName { get; }
    TenantSettingsModel Settings { get; }

    /// <summary>Los roles con permiso de captura; el resto solo consulta.</summary>
    bool CanWrite { get; }
    bool IsAdministrator { get; }

    ClaimsPrincipal BuildPrincipal(LoginResponse login);
}

public sealed class SessionContext(IHttpContextAccessor accessor) : ISessionContext
{
    public const string ApiTokenClaim = "fleet:api_token";
    public const string TenantNameClaim = "fleet:tenant_name";
    public const string SettingsClaim = "fleet:settings";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public string? ApiToken => Find(ApiTokenClaim);

    public string UserName => Find(ClaimTypes.Name) ?? "Usuario";

    public UserRole Role =>
        Enum.TryParse<UserRole>(Find(ClaimTypes.Role), out var role) ? role : UserRole.Viewer;

    public string TenantName => Find(TenantNameClaim) ?? "Mi empresa";

    public TenantSettingsModel Settings
    {
        get
        {
            var raw = Find(SettingsClaim);
            if (string.IsNullOrWhiteSpace(raw)) return TenantSettingsModel.Fallback();

            try
            {
                return JsonSerializer.Deserialize<TenantSettingsModel>(raw, Json) ?? TenantSettingsModel.Fallback();
            }
            catch (JsonException)
            {
                // Una cookie de una versión anterior no debe tumbar el portal.
                return TenantSettingsModel.Fallback();
            }
        }
    }

    public bool CanWrite => Role is UserRole.Administrator or UserRole.Dispatcher;

    public bool IsAdministrator => Role == UserRole.Administrator;

    public ClaimsPrincipal BuildPrincipal(LoginResponse login)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, login.UserId.ToString()),
            new(ClaimTypes.Name, login.FullName),
            new(ClaimTypes.Email, login.Email),
            new(ClaimTypes.Role, login.Role.ToString()),
            new(ApiTokenClaim, login.Token),
            new(TenantNameClaim, login.TenantName),
            new(SettingsClaim, JsonSerializer.Serialize(login.Settings, Json))
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "FleetCookie"));
    }

    private string? Find(string claimType) => User?.FindFirst(claimType)?.Value;
}

/// <summary>
/// Adjunta el token de la sesión a cada llamada saliente. Sin esto, cada
/// controlador tendría que acordarse de propagar la autenticación.
/// </summary>
public sealed class ApiTokenHandler(ISessionContext session) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = session.ApiToken;
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);
    }
}
