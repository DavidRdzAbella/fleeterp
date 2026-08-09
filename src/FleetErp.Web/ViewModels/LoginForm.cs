using System.ComponentModel.DataAnnotations;

namespace FleetErp.Web.ViewModels;

/// <summary>
/// Credenciales de acceso. La clave de la empresa se pide explícitamente porque
/// una misma instalación atiende a varias, y el correo por sí solo no basta para
/// saber a cuál pertenece el usuario.
/// </summary>
public sealed class LoginForm
{
    [Display(Name = "Empresa")]
    [Required(ErrorMessage = "Indique la clave de su empresa.")]
    public string TenantSlug { get; set; } = "demo";

    [Display(Name = "Correo")]
    [Required(ErrorMessage = "Capture su correo.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "Capture su contraseña.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Mantener la sesión abierta")]
    public bool RememberMe { get; set; } = true;
}
