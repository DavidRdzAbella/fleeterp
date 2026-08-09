using System.Globalization;

namespace FleetErp.Web.Services;

/// <summary>
/// Catálogos de zona horaria y formato regional para la pantalla de parámetros.
/// Se ofrecen como lista y no como texto libre porque un identificador mal
/// tecleado no falla al guardar: falla después, al formatear fechas, y en un
/// lugar lejano al error.
/// </summary>
public static class RegionalOptions
{
    /// <summary>
    /// Zonas de uso realista para un transportista en América. El identificador
    /// es el de Windows, que es lo que reconoce <see cref="TimeZoneInfo"/> en el
    /// equipo donde corre el portal.
    /// </summary>
    private static readonly (string Id, string Label)[] Curated =
    [
        ("Central Standard Time (Mexico)", "Centro de México — CDMX, Monterrey, Guadalajara"),
        ("Mountain Standard Time (Mexico)", "Pacífico de México — Chihuahua, Mazatlán"),
        ("Pacific Standard Time (Mexico)", "Noroeste — Tijuana, Ensenada"),
        ("US Mountain Standard Time", "Sonora y Arizona (sin horario de verano)"),
        ("SA Pacific Standard Time", "Colombia, Perú, Ecuador"),
        ("Central America Standard Time", "Centroamérica — Guatemala, San Salvador"),
        ("Central Standard Time", "Centro de Estados Unidos — Chicago, Dallas"),
        ("Mountain Standard Time", "Montaña de Estados Unidos — Denver"),
        ("Pacific Standard Time", "Pacífico de Estados Unidos — Los Ángeles"),
        ("Eastern Standard Time", "Este de Estados Unidos — Nueva York, Miami"),
        ("Argentina Standard Time", "Argentina — Buenos Aires"),
        ("E. South America Standard Time", "Brasil — São Paulo"),
        ("SA Western Standard Time", "Bolivia, Chile continental, Caracas"),
        ("UTC", "Tiempo universal coordinado")
    ];

    /// <summary>
    /// Zonas a mostrar. Solo se ofrecen las que el sistema operativo reconoce, y
    /// si la empresa tiene guardado un identificador que no está en la lista
    /// —por ejemplo uno con formato IANA— se agrega para no perderlo al guardar.
    /// </summary>
    public static IReadOnlyList<(string Id, string Label)> TimeZones(string? current)
    {
        var known = TimeZoneInfo.GetSystemTimeZones().Select(z => z.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var options = Curated
            .Where(z => known.Contains(z.Id))
            .ToList();

        if (options.Count == 0)
        {
            // Sistema con otro juego de identificadores (Linux con base IANA):
            // se listan los del propio sistema en lugar de dejar el combo vacío.
            options = TimeZoneInfo.GetSystemTimeZones()
                .OrderBy(z => z.BaseUtcOffset)
                .Select(z => (z.Id, $"{z.Id} ({z.BaseUtcOffset.TotalHours:+0.#;-0.#;0} h)"))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(current) &&
            !options.Any(z => string.Equals(z.Id, current, StringComparison.OrdinalIgnoreCase)))
        {
            options.Insert(0, (current, $"{current} (configurada actualmente)"));
        }

        return options;
    }

    /// <summary>Formatos regionales que cubren los mercados donde se vendería el sistema.</summary>
    public static IReadOnlyList<(string Code, string Label)> Locales(string? current)
    {
        var options = new List<(string Code, string Label)>
        {
            ("es-MX", "Español (México) — 1,234.56"),
            ("es-CO", "Español (Colombia)"),
            ("es-AR", "Español (Argentina) — 1.234,56"),
            ("es-CL", "Español (Chile)"),
            ("es-PE", "Español (Perú)"),
            ("es-ES", "Español (España) — 1.234,56"),
            ("es-GT", "Español (Guatemala)"),
            ("en-US", "Inglés (Estados Unidos)"),
            ("pt-BR", "Portugués (Brasil)")
        };

        if (!string.IsNullOrWhiteSpace(current) &&
            !options.Any(l => string.Equals(l.Code, current, StringComparison.OrdinalIgnoreCase)))
        {
            options.Insert(0, (current, $"{current} (configurado actualmente)"));
        }

        return options;
    }

    /// <summary>Ejemplo de cómo se verá una fecha con el formato elegido.</summary>
    public static string PreviewDate(string? locale)
    {
        try
        {
            return DateTime.Now.ToString("d 'de' MMMM 'de' yyyy, HH:mm", new CultureInfo(locale ?? "es-MX"));
        }
        catch (CultureNotFoundException)
        {
            return "—";
        }
    }
}
